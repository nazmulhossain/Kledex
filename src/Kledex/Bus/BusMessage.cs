using System.Collections.Generic;

namespace Kledex.Bus
{
    public abstract class BusMessage : IBusMessage
    {
        public IDictionary<string, object> Properties { get; set; }
    }
}
