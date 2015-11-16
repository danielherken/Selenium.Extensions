using System;
using System.Runtime.Serialization;

namespace Selenium.Extensions
{
    [Serializable]
    public class TestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestException"/> class.
        /// </summary>
        public TestException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public TestException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public TestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TestException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected TestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}