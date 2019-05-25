﻿using Kledex.Domain;

namespace Kledex.Tests.Fakes
{
    public class Aggregate : AggregateRoot
    {
        public Aggregate()
        {
            AddAndApplyEvent(new AggregateCreated());
        }

        private void Apply(AggregateCreated @event)
        {           
        }
    }
}