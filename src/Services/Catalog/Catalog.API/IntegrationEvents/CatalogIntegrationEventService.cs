using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using EventBus.Contracts;
using EventBus.Models;
using IntegrationEventLog;
using IntegrationEventLog.Contracts;
using IntegrationEventLog.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Catalog.API.IntegrationEvents
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly CatalogDbContext _catalogContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;

        public CatalogIntegrationEventService(
            IEventBus eventBus,
            CatalogDbContext catalogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            this._eventBus = eventBus;
            this._catalogContext = catalogContext;
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventLogService = _integrationEventLogServiceFactory(_catalogContext.Database.GetDbConnection());
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_catalogContext)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                    await _catalogContext.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt, _catalogContext.Database.CurrentTransaction);

                });
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
                Log.Information("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", evt.Id, Program.AppName, evt);
                _eventBus.Publish(evt);
                await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", evt.Id, Program.AppName, evt);
            }
        }
    }
}
