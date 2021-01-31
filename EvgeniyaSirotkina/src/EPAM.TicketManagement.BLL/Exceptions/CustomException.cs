using System;
using System.Runtime.Serialization;

namespace EPAM.TicketManagement.BLL.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CustomException(string message)
            : base(message)
        {
        }

        protected CustomException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
