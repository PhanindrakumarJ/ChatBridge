using ChatBridge.Chat.Application.DTOS;
using ChatBridge.Chat.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBridge.Chat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorProfileProvider _vendorProfileProvider;

        public VendorController(
            IVendorProfileProvider vendorProfileProvider)
        {
            _vendorProfileProvider = vendorProfileProvider;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<VendorProfileDto>> GetProfile(
            CancellationToken cancellationToken)
        {
            try
            {

            VendorProfileDto profile = await _vendorProfileProvider
                .GetProfileAsync(cancellationToken);

            return Ok(profile);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
