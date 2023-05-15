using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Hosting;

namespace Navyblue.Extensions.RabbitMQ.HostService
{
    public class RabbitMqListeningService : IHostedService, IDisposable
    {
        private readonly IComponentContext _container;

        public RabbitMqListeningService(IComponentContext container)
        {
            this._container = container;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var allConsumerTypes = _container.ComponentRegistry.Registrations
                .SelectMany(p => p.Services)
                .OfType<IServiceWithType>()
                .Where(p => p.ServiceType.IsInterface
                            && p.ServiceType.IsGenericType 
                            && p.ServiceType.GetGenericTypeDefinition() == typeof(IRabbitMqConsumer<>)).ToList();

            foreach (var consumerServiceType in allConsumerTypes.Select(consumer => this._container.Resolve(consumer.ServiceType)))
            {
                EnsureStart(() => ((dynamic) consumerServiceType).StartListening(), cancellationToken).ConfigureAwait(false);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        private async Task EnsureStart(Action action, CancellationToken cancellationToken)
        {
            bool connected = false;
            while (!connected && !cancellationToken.IsCancellationRequested)
            {
                lock (this)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            action();
                            connected = true;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                if (!connected)
                {

                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
            }
        }
    }
}