namespace Navyblue.Extensions.RabbitMQ
{
    public interface IRabbitMqConsumer<T>
    {
        void StartListening();
    }
}