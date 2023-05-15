using MediatR;

namespace RabbitMQSample.Model
{
    public class Queue2Message: IRequest<Unit>
    {
        public string Queue2Handler { get; set; }
    }
}