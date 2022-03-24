using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ConfigController : PeperinoController
    {
        [HttpGet]
        public ActionResult<bool> Get()
        {
            return Ok(true);
        }
    }
}
