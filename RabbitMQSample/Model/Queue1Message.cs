using MediatR;

namespace RabbitMQSample.Model
{
    public class Queue1Message: IRequest<Unit>
    {
        public string Queue1Handler { get; set; }
    }
}