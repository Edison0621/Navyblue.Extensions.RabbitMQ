using System.Collections.Generic;

namespace Navyblue.Extensions.RabbitMQ
{
    public interface IRabbitMqProducer<T>
    {
        void Publish(string message, Dictionary<string, object> headers);
    }
}