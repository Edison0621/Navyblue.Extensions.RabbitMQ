using System;

namespace Navyblue.Extensions.RabbitMQ
{
    public class NullGenericArgumentException : Exception
    {
        public NullGenericArgumentException(string message) : base(message)
        {

        }
    }
}