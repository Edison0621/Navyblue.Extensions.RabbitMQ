using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Navyblue.Extensions.RabbitMQ;
using RabbitMQSample.Model;

namespace RabbitMQSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder().UseConsoleLifetime()
                .ConfigureAppConfiguration(p =>
                {
                    p.AddXmlFile("RabbitMQ.Config");
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddMediatR(typeof(Queue1Message));
                    services.AddRabbitMQService(host);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory((builder) => { }))
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterGenericRabbitMQProducer();

                    builder.RegisterType<RabbitMqConsumer<Queue1Message>>().As<IRabbitMqConsumer<Queue1Message>>().SingleInstance();
                    builder.RegisterType<RabbitMqConsumer<Queue2Message>>().As<IRabbitMqConsumer<Queue2Message>>().SingleInstance();
                });

            hostBuilder.Build().Run();

            Console.ReadLine();
        }
    }
}
