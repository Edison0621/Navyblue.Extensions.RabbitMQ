using MediatR;

namespace RabbitMQSample.Model
{
    public class Queue1Message: IRequest
    {
        public string Queue1Handler { get; set; }
    }
}