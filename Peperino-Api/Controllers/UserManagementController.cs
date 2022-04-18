using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Peperino_Api.Helpers;
using Peperino_Api.Models.Request;
using Peperino_Api.Models.User;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserManagementController : PeperinoController
    {
        private readonly IUserManagementService userService;
        private readonly UserValidator validator;
        private readonly ILogger<UserManagementController> logger;

        public UserManagementController(IUserManagementService userService, UserValidator validator, ILogger<UserManagementController> logger)
        {
            this.userService = userService;
            this.validator = validator;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateNewUser(CreateUserRequest createUserRequest)
        {
            // Basic input validation
            this.validator.ValidateAndThrow(createUserRequest.User);
            var model = createUserRequest.User.AdaptToUser();

            if (model is not null)
            {
                // Create peperino user
                try
                {
                    var newUser = await this.userService.CreateNewUser(model, createUserRequest.Email, createUserRequest.Password);
                    return CreatedAtRoute(nameof(GetUserById), new { newUser.Id }, newUser.AdaptToDto());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest();
        }

        [HttpGet("data/{id:length(24)}", Name = nameof(GetUserById))]
        [PeperinoAuthorize("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var user = await this.userService.GetById(new ObjectId(id));

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
            if (this.PeperinoUser is not null)
            {
                var user = this.PeperinoUser.AdaptToDto();
                if (user is not null)
                {
                    return Ok(user);
                }
            }

            return BadRequest();
        }

        [HttpGet("data/{username}", Name = nameof(GetUserByUsername))]
        [PeperinoAuthorize]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            var user = await this.userService.GetByUsername(username);

            if (user is not null)
            {
                return Ok(user.AdaptToDto());
            }

            logger.LogWarning("username {username} not found", username);
            return NotFound();
        }

        [HttpPost("check", Name = nameof(CheckUsername))]
        public async Task<ActionResult<bool>> CheckUsername([FromBody] string username)
        {
            return Ok(await this.userService.CheckUsername(username));
        }

        [HttpPost("provider", Name = nameof(HandleProviderLogin))]
        [PeperinoAuthorize]
        public async Task<ActionResult<UserDto>> HandleProviderLogin(UserDto user)
        {
            this.validator.ValidateAndThrow(user);
            var model = user.AdaptToUser();

            var peperinoUser = await userService.HandleProviderLogin(model);

            return Ok(peperinoUser.AdaptToDto());
        }

        [HttpDelete("{externalId}")]
        [PeperinoAuthorize]
        public async Task<ActionResult> DeleteUser(string externalId, [FromServices] FirebaseApp firebase)
        {
            if( this.PeperinoUser?.ExternalId != externalId)
            {
                return BadRequest("Can't delete other users ;-)");
            }

            await FirebaseAuth.GetAuth(firebase).DeleteUserAsync(externalId);

            var peperinoUser = await userService.GetByExternalId(externalId);
            await userService.RemoveAsync(peperinoUser.Id);
            return Ok();
        }
    }
}