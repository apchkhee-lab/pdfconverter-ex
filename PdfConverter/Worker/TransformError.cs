using System;
using System.Runtime.Serialization;

namespace PdfConverter.Worker
{
    [Serializable]
    internal class TransformError : Exception
    {
        public TransformError()
        {
        }

        public TransformError(string message) : base(message)
        {
        }

        public TransformError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransformError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


}