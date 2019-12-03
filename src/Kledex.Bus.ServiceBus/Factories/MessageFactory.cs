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
                    if (serviceBusMessage.GetType().GetProperty(prop.Key) != null)
                    {
                        Action<Message, object> setter = (Action<Message, object>)Delegate.CreateDelegate(
                                typeof(Action<Message, object>),
                                null,
                                typeof(Message).GetProperty(prop.Key).GetSetMethod());

                        setter(serviceBusMessage, message.Properties[prop.Key]);
                    }
                    else
                    {
                        serviceBusMessage.UserProperties.Add(prop.Key, prop.Value);
                    }
                }
            }

            serviceBusMessage.UserProperties.Add(new KeyValuePair<string, object>(AssemblyQualifiedNamePropertyName, message.GetType().AssemblyQualifiedName));

            return serviceBusMessage;
        }
    }
}