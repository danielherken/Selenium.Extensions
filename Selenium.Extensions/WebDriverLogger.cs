using System;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;
using Selenium.Extensions.Interfaces;
using Xunit.Abstractions;

namespace Selenium.Extensions
{
    internal class WebDriverLogger : Logger
    {
        private readonly ITestWebDriver _testWebDriver;
        private ITestOutputHelper _testOutputHelper;

        public WebDriverLogger(ITestWebDriver testWebDriver, ITestOutputHelper testOutputHelper)
            : base(testWebDriver, testOutputHelper)
        {
            _testWebDriver = testWebDriver;
            _testOutputHelper = testOutputHelper;
        }

        //public override void TakeScreenshots()
        //{
        //    if (!_testWebDriver.Settings.LogScreenShots)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        Screenshot img = ((ITakesScreenshot)_testWebDriver).GetScreenshot();
        //        img.SaveAsFile(Path.Combine(_testWebDriver.Settings.TestDirectory + "Screenshots\\", string.Format("{0}.png", ScreenshotCounter.ToString("D5"))), ImageFormat.Png);
        //        ScreenshotCounter++;
        //    }
        //    catch (Exception ex)
        //    {
        //        Append("Errors", "Unable to capture screenshot");
        //        Append("Errors", ex.ToString());
        //    }
        //}

        //public override void DisplayMessageInBrowser(string message, int interval = 2500)
        //{
        //    //var messagePoster = new MessagePoster(_core);
        //    //messagePoster.DisplayMessageInBrowser(message, interval);
        //}
    }
}