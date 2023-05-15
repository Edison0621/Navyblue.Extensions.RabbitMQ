namespace Navyblue.Extensions.RabbitMQ.Setting
{
    public class RabbitMqQueueSetting
    {
        public string Exchange { get; set; }

        public string ExchangeType { get; set; }

        public string Name { get; set; }

        public ushort PrefetchCount { get; set; }

        public bool Durable { get; set; }

        public string RoutingKey { get; set; }

        public string MessageType { get; set; }
    }
}