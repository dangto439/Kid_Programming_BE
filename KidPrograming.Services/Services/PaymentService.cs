using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Http;

namespace KidPrograming.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Authentication _authentication;
        private readonly FcmService _fcmService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, Authentication authentication, FcmService fcmService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
            _fcmService = fcmService;
        }

        public async Task<string> CreateAsync(decimal amount, DateTimeOffset paymentDate)
        {
            Payment payment = new Payment()
            {
                Amount = amount,
                PaymentDate = paymentDate,
                Status = Enums.StatusPayment.Success.ToString(),
                CreatedTime = CoreHelper.SystemTimeNow,
            };

            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            await _unitOfWork.SaveAsync();

            return payment.Id;
        }
    }
}