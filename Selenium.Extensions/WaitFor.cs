using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Extensions
{
    public static class WaitFor
    {
        /// <summary>
        ///     Waits for the element to be present present with a specified time.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="timeSpan">The time span.</param>
        public static void ElementPresent(IWebDriver browser, By locator, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.FromSeconds(10);
            }
            Wait(browser, locator, (TimeSpan) timeSpan);
        }

        /// <summary>
        /// Waits the specified browser.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="timespan">The timespan.</param>
        private static void Wait(IWebDriver browser, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(browser, timespan);
            wait.Until(d => d.FindElement(locator));
        }
    }
}