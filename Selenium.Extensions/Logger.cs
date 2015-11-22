using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Selenium.Extensions.Interfaces;
using Xunit.Abstractions;

namespace Selenium.Extensions
{
    internal abstract class Logger : ILogger
    {
        private readonly ITestWebDriver _testWebDriver;

        private readonly ITestOutputHelper _testOutputHelper;


        /// <summary>
        /// Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="testWebDriver">The Test Web driver.</param>
        /// <param name="testOutputHelper">The test output helper.</param>
        protected Logger(ITestWebDriver testWebDriver, ITestOutputHelper testOutputHelper)
        {
            _testWebDriver = testWebDriver;
            _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Appends to the log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void AppendLogEntry(LogLevel logLevel, string message)
        {
            switch (_testWebDriver.Settings.LogLevel)
            {
                case LogLevel.None:
                    return;
                case LogLevel.Verbose:
                    _testOutputHelper.WriteLine(message + " - " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                    break;
                case LogLevel.Basic:
                    if (logLevel == LogLevel.Basic)
                    {
                        _testOutputHelper.WriteLine(message + " - " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                    }
                    break;
            }
        }
    }

}