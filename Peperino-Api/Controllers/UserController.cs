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
    public class UserController : PeperinoController
    {
        private readonly IUserService userService;
        private readonly UserValidator validator;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService userService, UserValidator validator, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.validator = validator;
            this.logger = logger;
        }

        [HttpPost]
        [PeperinoAuthorize] // Cant use this here because there is no user yet...
        public async Task<ActionResult<UserDto>> CreateNewUser(UserDto userDto)
        {
            // Basic input validation
            this.validator.ValidateAndThrow(userDto);

            // User can only create self
            if(userDto.ExternalId != this.FirebaseUser?.Uid)
            {
                logger.LogError("Given external id ({ExternalId}) does not match the external id from the current user ({ExternalId}): {userDto}", userDto.ExternalId, FirebaseUser?.Uid, userDto.ToJson());
                return this.UnprocessableEntity(userDto);
            }

            var model = userDto.AdaptToUser();

            if (model is not null)
            {
                // Check if a user with this username or external id already exists
                var exists = await this.userService.Exists(model);

                if (exists)
                {
                    logger.LogError("Username or external id is already in use: {model}", model.ToJson());
                    return this.BadRequest(userDto);
                }

                await this.userService.CreateAsync(model);
                return CreatedAtRoute(nameof(GetUserById), new { model.Id }, model.AdaptToDto());
            }

            return BadRequest();
        }

        [HttpGet("{id}", Name = nameof(GetUserById))]
        [PeperinoAuthorize("{id}")]
        public ActionResult<UserDto> GetUserById(string id)
        {
            var user = this.userService.GetById(new ObjectId(id)).Result;

            if (user is not null)
            {
                return Ok(user.AdaptToDto());
            }

            logger.LogWarning("id {id} not found", id);
            return NotFound();
        }

        [HttpGet]
        [PeperinoAuthorize]
        public ActionResult<UserDto> GetCurrentUser()
        {
            if(this.PeperinoUser is not null)
            {
                var user = this.PeperinoUser.AdaptToDto();
                if (user is not null)
                {
                    return Ok(user);
                }
            }

            return BadRequest();
        }
    }
}
