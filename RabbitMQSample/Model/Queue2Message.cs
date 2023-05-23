using MediatR;

namespace RabbitMQSample.Model
{
    public class Queue2Message: IRequest
    {
        public string Queue2Handler { get; set; }
    }
}