using KidPrograming.Contract.Services.Interfaces;
using KidProgramming.ModelViews.ModelViews.PaymentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ICacheService _cacheService;

        public VNPayController(IVnPayService vnPayService, ICacheService cacheService)
        {
            _vnPayService = vnPayService;
            _cacheService = cacheService;
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
            if (response.Success)
            {
                 await _cacheService.RemoveCacheResponseAsync("/api/dashboards/revenue");
                await _cacheService.RemoveCacheResponseAsync("/api/dashboards/top-course");
                return Redirect("https://www.youtube.com/results?search_query=setup+vnpay+v%E1%BB%9Bi+asp.net");
            }
            else
            {
                return Redirect("https://www.facebook.com/");
            }
        }
    }
}
