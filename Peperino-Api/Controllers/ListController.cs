using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Peperino_Api.Helpers;
using Peperino_Api.Models;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ListController : UserDependentController
    {
        private readonly IListService itemService;

        public ListController(IListService itemService)
        {
            this.itemService = itemService;
        }

        [HttpGet("{id}")]
        [FirebaseAuthorize]
        public async Task<ActionResult<List>> Get(string id)
        {
            var item = await this.itemService.GetById(GetCurrentUser(), new ObjectId(id));

            if (item is not null)
            {
                return Ok(item);
            }

            return NotFound();
        }

        [HttpPost]
        [FirebaseAuthorize]
        public async Task<ActionResult<string>> Post(List item)
        {
            var id = await this.itemService.Create(GetCurrentUser(), item);
            return Ok(id);
        }
    }
}
