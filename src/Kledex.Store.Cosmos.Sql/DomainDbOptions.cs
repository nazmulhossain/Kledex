﻿using Microsoft.Azure.Documents.Client;

namespace Kledex.Store.Cosmos.Sql
{
    public class DomainDbOptions
    {
        public string DatabaseId { get; set; } = "DomainStore";
        public string AggregateCollectionId { get; set; } = "Aggregates";
        public string CommandCollectionId { get; set; } = "Commands";
        public string EventCollectionId { get; set; } = "Events";
        public object PartitionKey { get; set; }
        public RequestOptions RequestOptions { get; set; } = new RequestOptions();
    }
}
