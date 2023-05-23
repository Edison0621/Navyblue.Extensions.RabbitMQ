using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.Extensions.RabbitMQ.HostService;
using Navyblue.Extensions.RabbitMQ.Setting;

namespace Navyblue.Extensions.RabbitMQ
{
    public static class RabbitMqExtension
    {
        public static void AddRabbitMqService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqSetting>(configuration.GetSection("RabbitMqSetting"));
            services.AddHostedService<RabbitMqListeningService>();
            services.AddAutofac();
        }

        public static void RegisterGenericRabbitMqProducer(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RabbitMqProducer<>)).As(typeof(IRabbitMqProducer)).InstancePerLifetimeScope();
        }
    }
}