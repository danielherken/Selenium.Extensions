using System.Collections.Generic;

namespace Selenium.Extensions.Interfaces
{
    public interface ILogger
    {
        /// <summary>
        /// Appends the log entry.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        void AppendLogEntry(LogLevel level, string message);

    }
}