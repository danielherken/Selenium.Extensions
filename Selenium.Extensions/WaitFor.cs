using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Extensions
{
    public static class WaitFor
    {
        public static void ElementPresent(IWebDriver browser, By locator)
        {
            Wait(browser, locator, TimeSpan.FromSeconds(10));
        }

        public static void ElementPresent(IWebDriver browser, By locator, TimeSpan timeSpan)
        {
            Wait(browser, locator, timeSpan);
        }

        private static void Wait(IWebDriver browser, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(browser, timespan);
            wait.Until(d => d.FindElement(locator));
        }
    }
}
