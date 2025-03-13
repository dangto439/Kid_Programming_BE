using KidPrograming.Contract.Services.Interfaces;
using KidProgramming.ModelViews.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace KidPrograming.Controllers
{
    [Route("api/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;

        public VNPayController(IVnPayService vnPayService, ICacheService cacheService, IConfiguration configuration)
        {
            _vnPayService = vnPayService;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        [HttpPost("payment-url")]
        public IActionResult CreatePaymentUrl([FromBody] PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(new { paymentUrl = url });
        }

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = await _vnPayService.PaymentExecute(Request.Query);
            var successUrl = _configuration["Vnpay:SuccessUrl"];
            var errorUrl = _configuration["Vnpay:ErrorUrl"];

            var queryParams = new Dictionary<string, string?>
                {
                    { "vnp_TransactionNo", response.TransactionId },
                    { "vnp_Amount", response.Amount.ToString() },
                    { "vnp_ResponseCode", response.VnPayResponseCode }
                };

            if (response.Success)
            {
                await _cacheService.RemoveCacheResponseAsync("/api/dashboards/revenue");
                await _cacheService.RemoveCacheResponseAsync("/api/dashboards/top-course");
                var redirectUrl = QueryHelpers.AddQueryString(successUrl, queryParams);
                return Redirect(redirectUrl);
            }
            else
            {
                var redirectUrl = QueryHelpers.AddQueryString(errorUrl, queryParams);
                return Redirect(redirectUrl);
            }
        }
    }
}
