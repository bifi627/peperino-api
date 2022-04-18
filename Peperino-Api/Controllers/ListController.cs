using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Peperino_Api.Helpers;
using Peperino_Api.Models.List;
using Peperino_Api.Models.Request;
using Peperino_Api.Services;

namespace Peperino_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ListController : PeperinoController
    {
        private readonly IListService listService;
        private readonly ListValidator validator;
        private readonly ListItemValidator itemValidator;

        public ListController(IListService itemService, ListValidator validator, ListItemValidator itemValidator)
        {
            this.listService = itemService;
            this.validator = validator;
            this.itemValidator = itemValidator;
        }

        [HttpPost]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListDto>> CreateNewList(ListDto list)
        {
            validator.ValidateAndThrow(list);

            var model = list.AdaptToList();

            if (model is not null && this.PeperinoUser is not null)
            {
                var originalSlug = list.Slug;
                var counter = 1;

                // TODO: Should probably just append a guid

                while (!await this.listService.CheckSlugAvailable(model.Slug))
                {
                    model.Slug = $"{originalSlug}-{counter}";
                    counter++;
                }

                var id = await this.listService.Create(this.PeperinoUser, model);

                if (id is not null)
                {
                    return Ok(list);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListDto[]>> GetLists()
        {
            if (this.PeperinoUser is not null)
            {
                var lists = await this.listService.GetAllForUser(this.PeperinoUser);

                return Ok(lists.ToArray());
            }
            return BadRequest();
        }

        [HttpGet("{slug}")]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListDto>> GetListBySlug(string slug)
        {
            if (this.PeperinoUser != null)
            {
                var list = await this.listService.GetBySlug(this.PeperinoUser, slug);

                if (list is not null)
                {
                    return Ok(list);
                }

                return NotFound();
            }
            return BadRequest();
        }

        [HttpPost("{slug}/text")]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListItemDto>> AddTextItemToList(string slug, [FromBody] string item)
        {
            if (this.PeperinoUser is not null)
            {
                var result = await this.listService.AddTextItem(this.PeperinoUser, slug, item);

                if (result is not null)
                {
                    return Ok(result.AdaptToDto());
                }

                return BadRequest("Failed to add item to list");
            }
            return BadRequest();
        }

        [HttpPut("{slug}/{itemId}")]
        [PeperinoAuthorize]
        public async Task<ActionResult<bool>> UpdateItem(string slug, string itemId, [FromBody] ListItemDto listItemDto)
        {
            if (this.PeperinoUser is not null)
            {
                itemValidator.ValidateAndThrow(listItemDto);

                var model = listItemDto.AdaptToListItem();

                if (model is not null)
                {
                    var result = await this.listService.UpdateItem(this.PeperinoUser, slug, model);
                    return Ok(result);
                }

            }
            return BadRequest();
        }

        [HttpDelete("{slug}/{itemId}")]
        [PeperinoAuthorize]
        public async Task<ActionResult<bool>> DeleteItem(string slug, Guid itemId)
        {
            if (this.PeperinoUser is not null)
            {
                var result = await this.listService.DeleteItem(PeperinoUser, slug, itemId);

                if (result)
                {
                    return Ok(true);
                }
            }

            return BadRequest();
        }

        [HttpPost("{slug}/move")]
        [PeperinoAuthorize]
        public async Task<ActionResult<ListDto>> MoveItem(string slug, [FromBody] MoveItemRequest request)
        {
            if (this.PeperinoUser is not null)
            {
                var result = await this.listService.MoveItem(this.PeperinoUser, slug, request.From, request.To);

                if (result is not null)
                {
                    var dto = result.AdaptToDto();
                    return Ok(dto);
                }
            }
            return BadRequest();
        }
    }
}
