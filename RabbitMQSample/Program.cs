using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navyblue.Extensions.RabbitMQ;
using RabbitMQSample.Model;

namespace RabbitMQSample
{
    class Program
    {
        static void Main(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder().UseConsoleLifetime()
                .ConfigureAppConfiguration(p =>
                {
                    p.AddXmlFile("RabbitMQ.Config");
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddMediatR(msc=>msc.RegisterServicesFromAssembly(typeof(Queue1Message).GetTypeInfo().Assembly));
                    services.AddRabbitMqService(host.Configuration);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(builder => { }))
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterGenericRabbitMqProducer();

                    builder.RegisterType<RabbitMqConsumer<Queue1Message>>().As<IRabbitMqConsumer>().SingleInstance();
                    builder.RegisterType<RabbitMqConsumer<Queue2Message>>().As<IRabbitMqConsumer>().SingleInstance();
                });

            hostBuilder.Build().Run();

            Console.ReadLine();
        }
    }
}
