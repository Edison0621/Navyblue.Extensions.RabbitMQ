using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Navyblue.BaseLibrary;
using RabbitMQSample.Model;

namespace RabbitMQSample.Handler
{
    public class Queue1Handler : IRequestHandler<Queue1Message>
    {
        public Task Handle(Queue1Message request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received {nameof(Queue1Message)}: {request.ToJson()}");

            return Task.FromResult(new Unit());
        }
    }
}