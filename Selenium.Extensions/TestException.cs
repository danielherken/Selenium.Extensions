using System;
using System.Runtime.Serialization;

namespace Selenium.Extensions
{
    [Serializable]
    public class TestException : Exception
    {
        public TestException()
            : base()
        { }

        public TestException(string message)
            : base(message)
        { }

        public TestException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public TestException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public TestException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        { }

        protected TestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
