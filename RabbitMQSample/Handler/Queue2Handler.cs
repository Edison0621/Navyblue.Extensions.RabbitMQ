using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Navyblue.BaseLibrary;
using RabbitMQSample.Model;

namespace RabbitMQSample.Handler
{
    public class Queue2Handler : IRequestHandler<Queue2Message>
    {
        public Task<Unit> Handle(Queue2Message request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received {nameof(Queue2Message)}: {request.ToJson()}");

            return Task.FromResult(new Unit());
        }
    }
}