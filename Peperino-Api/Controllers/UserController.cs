using Microsoft.AspNetCore.Mvc;
using Peperino_Api.Models;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return this.userService.Get();
        }
    }
}
