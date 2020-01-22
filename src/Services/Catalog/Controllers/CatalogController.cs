using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalog.Models.DomainModels;
using System.Net;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ILogger<CatalogController> logger)
        {
            _logger = logger;
        }

        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public IActionResult CreateItem([FromBody] CatalogItem itemDto)
        {
            var item = new CatalogItem
            {
                BrandId = itemDto.BrandId,
                TypeId = itemDto.TypeId,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price
            };
            return Ok();
        }
    }
}
