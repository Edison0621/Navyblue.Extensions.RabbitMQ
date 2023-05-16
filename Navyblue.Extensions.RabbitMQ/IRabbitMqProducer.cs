using System.Collections.Generic;

namespace Navyblue.Extensions.RabbitMQ
{
    public interface IRabbitMqProducer
    {
        void Publish(string message, Dictionary<string, object> headers);
    }
}