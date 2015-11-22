using System;

namespace Selenium.Extensions.Exceptions
{
    [Serializable]
    public class DriverNotFoundException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotFoundException"/> class.
        /// </summary>
        public DriverNotFoundException()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DriverNotFoundException(string message) : base(message)
		{
		}
	}
}