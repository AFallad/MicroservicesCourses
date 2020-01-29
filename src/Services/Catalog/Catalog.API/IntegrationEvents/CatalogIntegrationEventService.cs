using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using EventBus.Contracts;
using EventBus.Models;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Catalog.API.IntegrationEvents
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly CatalogDbContext _catalogContext;

        public CatalogIntegrationEventService(
            IEventBus eventBus,
            CatalogDbContext catalogContext)
        {
            this._eventBus = eventBus;
            this._catalogContext = catalogContext;
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                Log.Information("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", evt.Id, Program.AppName, evt);

                _eventBus.Publish(evt);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", evt.Id, Program.AppName, evt);
            }
        }
    }
}
