using System.Collections.Generic;

namespace Navyblue.Extensions.RabbitMQ.Setting
{
    public class RabbitMqSetting
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public bool SslEnabled { get; set; }

        public string CertSubject { get; set; }

        public bool AutoAck { get; set; }

        public int WaitForConfirmsMilliseconds { get; set; }

        public bool Persistent { get; set; }

        public List<RabbitMqQueueSetting> Queues { get; set; }
    }
}