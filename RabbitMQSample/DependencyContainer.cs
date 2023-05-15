using System.Reflection;
using Autofac;
using MediatR;
using RabbitMQSample.Model;
using RabbitMQSample.RabbitMq;

namespace RabbitMQSample
{
    public class DependencyContainer
    {
        public static void UseAutoFacContainer(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RabbitMqProducer<>)).As(typeof(IRabbitMqProducer<>)).InstancePerLifetimeScope();

            builder.RegisterType<RabbitMqConsumer<Queue1Message>>().As<IRabbitMqConsumer<Queue1Message>>().SingleInstance();
            builder.RegisterType<RabbitMqConsumer<Queue2Message>>().As<IRabbitMqConsumer<Queue2Message>>().SingleInstance();
        }
    }
}