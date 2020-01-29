using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventBus.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntegrationEventLog.Contracts
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
