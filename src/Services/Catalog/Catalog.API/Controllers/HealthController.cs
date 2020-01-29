using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.IntegrationEvents;
using Catalog.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public HealthController(ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            this._catalogIntegrationEventService = catalogIntegrationEventService;
        }
        [HttpGet]
        [Route("isAlive")]
        public IActionResult IsAlive()
        {
            var priceChangedEvent = new ProductPriceChangedIntegrationEvent(1, 10, 12);
            _catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            return this.NoContent();
        }


    }
}
