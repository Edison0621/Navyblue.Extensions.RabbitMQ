using System;
using System.Diagnostics;
using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using Navyblue.BaseLibrary;
using Navyblue.Extensions.RabbitMQ.Setting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Navyblue.Extensions.RabbitMQ
{
    public class RabbitMqConsumer<T> : RabbitMqBase, IRabbitMqConsumer
    {
        private readonly RabbitMqSetting _setting;
        private readonly IMediator _mediator;

        public RabbitMqConsumer(IOptions<RabbitMqSetting> setting, IMediator mediator) : base(setting)
        {
            this._setting = setting.Value;
            this._mediator = mediator;
        }

        public void StartListening()
        {
            RabbitMqQueueSetting queueSetting = this._rabbitMqQueueSettings[typeof(T).FullName ?? throw new NullGenericArgumentException("Missing generic argument full name in RabbitMqConsumer")];

            IConnection connection = this.GetConnection(this._setting.Host);

            IModel model = connection.CreateModel();
            model.BasicQos(0, queueSetting.PrefetchCount, true);

            model.ExchangeDeclare(queueSetting.Exchange, queueSetting.ExchangeType, queueSetting.Durable);
            model.QueueDeclare(queueSetting.Name, true, false, false);
            model.QueueBind(queueSetting.Name, queueSetting.Exchange, queueSetting.RoutingKey);

            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            consumer.Received += this.OnMessageReceived;

            // T0 identify the consumer tag of this subscription
            string consumerTag = Environment.MachineName + "." + Process.GetCurrentProcess().Id + "." + queueSetting.Name;
            model.BasicConsume(queueSetting.Name, this._setting.AutoAck, consumerTag, consumer);
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            EventingBasicConsumer consumer = sender as EventingBasicConsumer;
            if (consumer == null)
            {
                throw new Exception("Exception occurred");
            }

            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            T data = message.FromJson<T>();

            this._mediator.Send(data);

            string replyToMessage = "This is response message";

            if (!string.IsNullOrEmpty(eventArgs.BasicProperties.ReplyTo))
            {
                this.ReplyTo(consumer.Model, eventArgs.BasicProperties.ReplyTo, eventArgs.BasicProperties.CorrelationId, Encoding.UTF8.GetBytes(replyToMessage));
            }

            if (!this._setting.AutoAck)
            {
                consumer.Model.BasicAck(eventArgs.DeliveryTag, false);
            }
        }

        private void ReplyTo(IModel model, string replyTo, string correlationId, byte[] responseBuffer)
        {
            IBasicProperties responseProperties = model.CreateBasicProperties();

            responseProperties.CorrelationId = correlationId;

            model.BasicPublish("", replyTo, responseProperties, responseBuffer);
        }
    }
}