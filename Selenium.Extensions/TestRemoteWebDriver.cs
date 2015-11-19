using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Selenium.Extensions
{
    public class TestRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
    {
        public TestRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities)
        {
        }

        public TestRemoteWebDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities)
        {
        }

        public TestRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
        }

        public TestRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout)
            : base(remoteAddress, desiredCapabilities, commandTimeout)
        {
        }

        Screenshot ITakesScreenshot.GetScreenshot()
        {
            var response = Execute(DriverCommand.Screenshot, null);
            var base64 = response.Value.ToString();
            return new Screenshot(base64);
        }
    }
}