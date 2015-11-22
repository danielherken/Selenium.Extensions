using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Extensions
{
    public static class WaitUntil
    {
        /// <summary>
        ///     Waits for the element to be present present with a optional time.
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
            WaitForElement(browser, locator, (TimeSpan) timeSpan);
        }

        /// <summary>
        /// Waits for the element to be presentr.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="timespan">The timespan.</param>
        private static void WaitForElement(IWebDriver browser, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(browser, timespan);
            wait.Until(d => d.FindElement(locator));
        }
    }
}