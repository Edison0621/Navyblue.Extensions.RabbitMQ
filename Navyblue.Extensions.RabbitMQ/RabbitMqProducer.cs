using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Options;
using Navyblue.Extensions.RabbitMQ.Setting;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Navyblue.Extensions.RabbitMQ
{
    public class RabbitMqProducer<T> : RabbitMqBase, IRabbitMqProducer
    {
        private readonly RabbitMqSetting _setting;
        private IModel _channel;

        public RabbitMqProducer(IOptions<RabbitMqSetting> setting) : base(setting)
        {
            this._setting = setting.Value;
        }

        public void Publish(string message, Dictionary<string, object> headers)
        {
            IConnection connection = this.GetConnection(this._setting.Host);

            RabbitMqQueueSetting queueSetting = this._rabbitMqQueueSettings[typeof(T).FullName ?? throw new NullGenericArgumentException("Missing generic argument full name in RabbitMqProducer")];

            if (this._channel == null || this._channel.IsClosed)
            {
                this._channel = connection.CreateModel();
            }

            this._channel.BasicQos(0, queueSetting.PrefetchCount, true);

            this._channel.ExchangeDeclare(queueSetting.Exchange, queueSetting.ExchangeType, queueSetting.Durable);
            this._channel.QueueDeclare(queueSetting.Name, true, false, false);
            this._channel.QueueBind(queueSetting.Name, queueSetting.Exchange, queueSetting.RoutingKey);

            this._channel.BasicReturn += (s, args) =>
            {
                //Handling Unroutable Messages
            };

            //_channel.BasicAcks += (s, args) =>
            //{
            //    Console.Write("ACK received when publishing message");
            //};

            this._channel.ConfirmSelect();

            IBasicProperties messageProperties = this._channel.CreateBasicProperties();
            messageProperties.Headers = headers;
            messageProperties.ContentType = "text/plain";
            messageProperties.DeliveryMode = 2;
            messageProperties.Persistent = true;

            byte[] body;
            if (typeof(T) == typeof(string))
            {
                string msg = Convert.ToString(message);
                body = Encoding.UTF8.GetBytes(msg);
            }
            else
            {
                JsonSerializer serializer = new JsonSerializer();
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    serializer.Serialize(sw, message);
                    body = ms.ToArray();
                    sw.Flush();
                }
            }

            this._channel.BasicPublish(
                queueSetting.Exchange,
                string.IsNullOrEmpty(queueSetting.RoutingKey) ? queueSetting.Name : "routingKey",
                basicProperties: messageProperties,
                mandatory: true,
                body: body);

            this._channel.WaitForConfirms(new TimeSpan(0, 0, 0, 0, this._setting.WaitForConfirmsMilliseconds));
        }
    }
}