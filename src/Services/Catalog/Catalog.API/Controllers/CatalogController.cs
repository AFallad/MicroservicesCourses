using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalog.API.Models.DomainModels;
using System.Net;
using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogDbContext _catalogContext;   

        public CatalogController(CatalogDbContext catalogContext)
        {
            this._catalogContext = catalogContext;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogItem>> ItemByIdAsync(int id)
        {
            var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                return this.Ok(item);
            }

            return NoContent();
        }

        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public IActionResult CreateItemAsync([FromBody] CatalogItem itemDto)
        {
            var item = new CatalogItem
            {
                BrandId = itemDto.BrandId,
                TypeId = itemDto.TypeId,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price
            };
            this._catalogContext.CatalogItems.Add(item);

            return CreatedAtAction(nameof(ItemByIdAsync), new {id = item.Id}, null);
        }
    }
}
