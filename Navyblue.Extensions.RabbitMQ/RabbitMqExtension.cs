using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navyblue.Extensions.RabbitMQ.HostService;
using Navyblue.Extensions.RabbitMQ.Setting;

namespace Navyblue.Extensions.RabbitMQ
{
    public static class RabbitMqExtension
    {
        public static void AddRabbitMQService(this IServiceCollection services, HostBuilderContext host)
        {
            services.Configure<RabbitMqSetting>(host.Configuration.GetSection("RabbitMqSetting"));
            services.AddHostedService<RabbitMqListeningService>();
            services.AddAutofac();
        }

        public static void RegisterGenericRabbitMQProducer(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RabbitMqProducer<>)).As(typeof(IRabbitMqProducer<>)).InstancePerLifetimeScope();
        }
    }
}