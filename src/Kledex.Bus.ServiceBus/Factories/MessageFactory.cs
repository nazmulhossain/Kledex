using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.ServiceBus;

namespace Kledex.Bus.ServiceBus.Factories
{
    public class MessageFactory : IMessageFactory
    {
        public static readonly string AssemblyQualifiedNamePropertyName = "AssemblyQualifiedName";

        /// <inheritdoc />
        public Message CreateMessage<TMessage>(TMessage message) where TMessage : IBusMessage
        {
            var json = JsonSerializer.Serialize(message);
            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(json))
            {
                ContentType = "application/json"
            };

            if (message.Properties != null)
            {
                foreach (var prop in message.Properties)
                {
                    var mProp = serviceBusMessage.GetType().GetProperty(prop.Key);
                    if (mProp != null)
                    {
                        //Type mPropoType = mProp.GetType();
                        mProp.SetValue(mProp, message.Properties[prop.Key]);
                    }
                    else
                    {
                        serviceBusMessage.UserProperties.Add(prop.Key, prop.Value);
                    }


                    // We could use reflexion here, but i believe we should bet on performace and simplicity.
                    // If not, then we can consider adding more of this properties
                    //if (prop.Key == nameof(serviceBusMessage.Label))
                    //    serviceBusMessage.Label = message.Properties[prop.Key] as string;
                    //else if (prop.Key == nameof(serviceBusMessage.SessionId))
                    //    serviceBusMessage.SessionId = message.Properties[prop.Key] as string;
                    //else if (prop.Key == nameof(serviceBusMessage.CorrelationId))
                    //    serviceBusMessage.CorrelationId = message.Properties[prop.Key] as string;
                    //else if (prop.Key == nameof(serviceBusMessage.ScheduledEnqueueTimeUtc) && message.Properties[prop.Key] is System.DateTime ScheduledEnqueueTimeUtc)
                    //    serviceBusMessage.ScheduledEnqueueTimeUtc = ScheduledEnqueueTimeUtc;
                    //else
                    //    serviceBusMessage.UserProperties.Add(prop.Key, prop.Value);
                }
            }

            serviceBusMessage.UserProperties.Add(new KeyValuePair<string, object>(AssemblyQualifiedNamePropertyName, message.GetType().AssemblyQualifiedName));

            return serviceBusMessage;
        }
    }
}