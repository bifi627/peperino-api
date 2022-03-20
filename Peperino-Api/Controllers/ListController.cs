using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Peperino_Api.Helpers;
using Peperino_Api.Models.List;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ListController : PeperinoController
    {
        private readonly IListService itemService;
        private readonly ListValidator validator;

        public ListController(IListService itemService, ListValidator validator)
        {
            this.itemService = itemService;
            this.validator = validator;
        }

        [HttpGet("{id}")]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListDto>> GetListById(string id)
        {
            var list = await this.itemService.GetById(this.PeperinoUser, new ObjectId(id));

            if (list is not null)
            {
                return Ok(list.AdaptToDto());
            }

            return NotFound();
        }

        [HttpPost]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListDto>> CreateNewList(ListDto list)
        {
            validator.ValidateAndThrow(list);

            var model = list.AdaptToList();

            if (model is not null)
            {
                var id = await this.itemService.Create(this.PeperinoUser, model);

                if (id is not null)
                {
                    return CreatedAtRoute(nameof(GetListById), new { id }, model.AdaptToDto());
                }
            }

            return BadRequest();
        }
    }
}
