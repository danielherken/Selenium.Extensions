using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Selenium.Extensions
{
    public class ScreenshotRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
    {
        public ScreenshotRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities)
        {
        }

        public ScreenshotRemoteWebDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities)
        {
        }

        public ScreenshotRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
        }

        public ScreenshotRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout)
            : base(remoteAddress, desiredCapabilities, commandTimeout)
        {
        }

        Screenshot ITakesScreenshot.GetScreenshot()
        {
            Response response = Execute(DriverCommand.Screenshot, null);
            string base64 = response.Value.ToString();
            return new Screenshot(base64);
        }
    }
}
