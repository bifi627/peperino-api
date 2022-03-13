using Microsoft.AspNetCore.Mvc;
using Peperino_Api.Helpers;
using Peperino_Api.Models;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("{userId}")]
        [FirebaseAuthorize("userId")]
        public Task<User> Get(string userId)
        {
            return this.userService.GetById(userId);
        }

        [HttpGet]
        [FirebaseAuthorize]
        public IEnumerable<User> Get()
        {
            return this.userService.GetAll();
        }
    }
}
