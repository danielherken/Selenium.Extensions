using System;

namespace Selenium.Extensions.Exceptions
{
    [Serializable]
    public class DriverBlockedException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverBlockedException"/> class.
        /// </summary>
        public DriverBlockedException()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverBlockedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DriverBlockedException(string message) : base(message)
		{
		}
	}
}