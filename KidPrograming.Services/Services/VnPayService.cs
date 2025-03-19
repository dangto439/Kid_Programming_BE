using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using KidProgramming.ModelViews.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Services.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Authentication _authentication;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly FcmService _fcmService;
        private readonly INotificationService _notificationService;

        public VnPayService(FcmService fcmService, IConfiguration configuration, Authentication authentication, IHttpContextAccessor httpContextAccessor, IPaymentService paymentService, IEnrollmentService enrollmentService,IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
            _paymentService = paymentService;
            _enrollmentService = enrollmentService;
            _unitOfWork = unitOfWork;
            _fcmService = fcmService;
            _notificationService = notificationService;
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            bool enrollmentExists = await _unitOfWork.GetRepository<Enrollment>().Entities.AnyAsync(e => e.UserId == userId && e.CourseId == model.CourseId && !e.DeletedTime.HasValue);

            if(enrollmentExists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, "Enrollment has enroll in this course");
            }

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Course:{model.CourseId}|User:{userId}|Desc:{model.OrderDescription}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public async Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            string amountStr = response.Amount;
            decimal amount = !string.IsNullOrEmpty(amountStr) ? decimal.Parse(amountStr) / 100 : 0;

            string paymentDateStr = response.PaymentDate;
            DateTimeOffset paymentDate = DateTimeOffset.UtcNow;
            if (!string.IsNullOrEmpty(paymentDateStr) && DateTimeOffset.TryParseExact(
                paymentDateStr, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset parsedDate))
            {
                paymentDate = parsedDate;
            }

            string courseId = ExtractCourseId(response.OrderDescription);
            string userId = ExtractUserId(response.OrderDescription);


            if (response.Success)
            {
                try
                {
                    string paymentId = await _paymentService.CreateAsync(amount, paymentDate);

                    await _enrollmentService.CreateEnrollment(userId, paymentId, courseId);

                    Course course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == courseId)
                        ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Course not found");
                    string courseTitle = course.Title;

                    var notificationSection = _configuration.GetSection("Notification");

                    if (!int.TryParse(notificationSection["Type"], out int typeValue))
                    {
                        throw new ArgumentException($"Invalid notification type value in config: {notificationSection["Type"]}");
                    }

                    if (!Enum.IsDefined(typeof(NotificationType), typeValue))
                    {
                        throw new ArgumentException($"Notification type {typeValue} is not defined in the enum.");
                    }

                    NotificationType notificationType = (NotificationType)typeValue;

                    CreateNotificationSingleModel model = new()
                    {
                        Title = notificationSection["Title"],
                        Message = string.Format(notificationSection["Message"], courseTitle),
                        Type = (NotificationType)typeValue,
                        ReceiverId = userId
                    };

                    await _notificationService.CreateNotificationSingle(model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return response;
        }

        private string ExtractCourseId(string orderInfo)
        {
            if (string.IsNullOrEmpty(orderInfo)) return string.Empty;

            var parts = orderInfo.Split('|');
            foreach (var part in parts)
            {
                if (part.StartsWith("Course:"))
                {
                    return part.Split(':')[1];
                }
            }
            return string.Empty;
        }

        private string ExtractUserId(string orderInfo)
        {
            if (string.IsNullOrEmpty(orderInfo)) return string.Empty;

            var parts = orderInfo.Split('|');
            foreach (var part in parts)
            {
                if (part.StartsWith("User:"))
                {
                    return part.Split(':')[1];
                }
            }
            return string.Empty;
        }
    }
}