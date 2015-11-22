using System;
using System.Runtime.Serialization;

namespace Selenium.Extensions.Exceptions
{
    [Serializable]
    public class TestConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxySeleniumBlockedException"/> class.
        /// </summary>
        public TestConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxySeleniumBlockedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TestConfigurationException(string message) : base(message)
		{
        }
    }
}