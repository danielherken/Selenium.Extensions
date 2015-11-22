namespace Selenium.Extensions.Interfaces
{

    public interface IDialogManager
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
        DialogManager SelectFiles(WebDriverType webDriverType, string directory, params string[] files);
    }
}