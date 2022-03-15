using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Peperino_Api.Helpers;
using Peperino_Api.Models.User;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly UserValidator validator;

        public UserController(IUserService userService, UserValidator validator)
        {
            this.userService = userService;
            this.validator = validator;
        }

        [HttpPost]
        [FirebaseAuthorize]
        public async Task<ActionResult<UserDto>> CreateNewUser(UserDto userDto)
        {
            this.validator.ValidateAndThrow(userDto);

            var model = userDto.AdaptToUser();

            if (model is not null)
            {
                await this.userService.CreateAsync(model);
                return CreatedAtRoute(nameof(GetUserById), new { model.Id }, model.AdaptToDto());
            }

            return BadRequest();
        }

        [HttpGet("{id}", Name = nameof(GetUserById))]
        [FirebaseAuthorize("{id}")]
        public ActionResult<UserDto> GetUserById(string id)
        {
            var user = this.userService.GetById(new ObjectId(id)).Result;

            if (user is not null)
            {
                return Ok(user.AdaptToDto());
            }

            return NotFound();
        }
    }
}
