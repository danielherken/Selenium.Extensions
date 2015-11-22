using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Selenium.Extensions.Interfaces;

namespace Selenium.Extensions
{
    /// <summary>
    ///     Manages dialog boxes.
    /// </summary>
    public class DialogManager : IDialogManager
    {
        /// <summary>
        /// Interacts with file dialog windows.
        /// </summary>
        /// <param name="webDriverType">The Type of web driver.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="files">The files.</param>
        /// <returns>
        /// The current Dialog Manager instance.
        /// </returns>
        /// <exception cref="System.Exception">Could not find dialog window.</exception>
        public DialogManager SelectFiles(WebDriverType webDriverType, string directory, params string[] files)
        {
            //_log.Append("Verbose", string.Format("Select {0} file(s) in directory: {1}", files.Length, directory.AbsolutePath()));

            string lookForTitle = null;
            switch (webDriverType)
            {
                case WebDriverType.ChromeDriver:
                    lookForTitle = "Open";
                    break;
                case WebDriverType.FirefoxDriver:
                    lookForTitle = "File Upload";
                    break;
                case WebDriverType.InternetExplorerDriver:
                    lookForTitle = "Choose File to Upload";
                    break;
            }

            bool success;
            int tries = 0;
            do
            {
                success = WindowHelper.GetDialogHandles(lookForTitle).Any();
                tries++;
                if (!success && tries <= 20)
                {
                    Thread.Sleep(250);
                }
            } while (!success && tries < 20);

            if (!success)
            {
                throw new Exception("Could not find dialog window.");
            }

            IEnumerable<IntPtr> dialogs = WindowHelper.GetDialogHandles(lookForTitle);
            IntPtr dialog = dialogs.FirstOrDefault();

            if (dialog != default(IntPtr))
            {
                // Set the window title to something unique so other WebManager instances don't try to use it
                string title = lookForTitle + " - "+ RandomData.RandomAlphanumeric(3);
                WindowHelper.SetWindowTitle(dialog, title);

                // Select files and submit
                DialogHelper.SelectFiles(dialog, title, directory, files);
            }

            return this;
        }
    }
}