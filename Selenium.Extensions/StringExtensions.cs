using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Selenium.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Returns the absolute path of the provided relative path.
        ///     If the path is already absolute, it returns the path without modification.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The absolute path.
        /// </returns>
        public static string AbsolutePath(this string value)
        {
            string absolutePath = value;
            if (Regex.IsMatch(value, @"^\\\\?.*|^[a-zA-Z](?!:).*"))
            {
                absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
            }

            return absolutePath;
        }
    }
}
