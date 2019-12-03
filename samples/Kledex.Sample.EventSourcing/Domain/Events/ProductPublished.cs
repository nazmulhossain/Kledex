using System;
using System.Collections.Generic;
using Kledex.Bus;
using Kledex.Domain;

namespace Kledex.Sample.EventSourcing.Domain.Events
{
    public class ProductPublished : DomainEvent, IBusQueueMessage
    {
        public string QueueName { get; set; } = "ProductPublishedQueue";

        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>()
        {
            { "ScheduledEnqueueTimeUtc", DateTime.UtcNow.AddMinutes(10) },
            { "CorrelationId", Guid.NewGuid() }
        };
    }
}
