using KidProgramming.ModelViews.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Http;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
    }
}
