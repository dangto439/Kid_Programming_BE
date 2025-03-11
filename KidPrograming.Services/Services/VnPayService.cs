using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace KidPrograming.Services.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Authentication _authentication;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly IEnrollmentService _enrollmentService;

        public VnPayService(IConfiguration configuration, Authentication authentication, IHttpContextAccessor httpContextAccessor, IPaymentService paymentService, IEnrollmentService enrollmentService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
            _paymentService = paymentService;
            _enrollmentService = enrollmentService;
        }
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            var userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

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
            //foreach (var (key, value) in collections)
            //{
            //    Console.WriteLine($"VNPay Response: Key = {key}, Value = {value}");
            //}

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
                string paymentId = await _paymentService.CreateAsync(amount, paymentDate);
                await _enrollmentService.CreateEnrollment(userId, paymentId, courseId);
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
