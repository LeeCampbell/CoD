using Microsoft.AspNetCore.Mvc;

namespace Yow.CoD.Finance.WebCommsAdapter.Controllers
{
    [Route("")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("")]
        public ActionResult Ping()
        {
            return Content("YOW 2017 - Cost Of a Dependency");
        }
    }
}
