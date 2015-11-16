using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Extensions
{
    public static class WebDriverExtension
    {
        public static string GetText(this IWebDriver driver)
        {
            return driver.FindElement(By.TagName("body")).Text;
        }

        public static IWebElement TryFindElement(this IWebDriver driver, By locator)
        {
            try
            {
                return driver.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public static bool HasElement(this IWebDriver driver, By locator)
        {
            try
            {
                driver.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        public static void WaitForPageToLoad(this IWebDriver driver, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.FromSeconds(30);
            }
            var wait = new WebDriverWait(driver, (TimeSpan) timeSpan);
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("Driver must support javascript execution", nameof(driver));
            }
            wait.Until(d =>
            {
                try
                {
                    var readyState = javascript.ExecuteScript(
                        "if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        ///     Waits until the element is displayed.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="timeout">The timeout.</param>
        public static void WaitUntiDisplayed(this IWebDriver driver, By locator, TimeSpan timeout)
        {
            new WebDriverWait(driver, timeout)
                .Until(d => d.FindElement(locator).Enabled
                            && d.FindElement(locator).Displayed
                            && d.FindElement(locator).GetAttribute("aria-disabled") == null
                );
        }

        /// <summary>
        ///     Waits until the element is visible.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="timeout">The timeout.</param>
        public static void WaitUntilVisible(this IWebDriver driver, By locator, TimeSpan timeout)
        {
            (new WebDriverWait(driver, timeout)).Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        ///     Scrolls down page.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentException">Web driver must support javascript execution</exception>
        public static void ScrollDownPage(this IWebDriver driver)
        {
            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("Web driver must support javascript execution", nameof(javascript));
            }
            javascript.ExecuteScript(
                "var body = document.body, html  = document.documentElement; var height = Math.max(body.scrollHeight,body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight); window.scrollBy(0, height)");
        }

        public static List<IWebElement> GetElementsOfType(this IWebDriver driver, ElementType elementType)
        {
            var webElements = new List<IWebElement>();
            switch (elementType)
            {
                case ElementType.Button:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "button"));
                }
                    break;
                case ElementType.CheckBox:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "checkbox"));
                }
                    break;
                case ElementType.Div:
                {
                    var children = driver.FindElements(By.TagName("div"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Img:
                {
                    var children = driver.FindElements(By.TagName("img"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Label:
                {
                    var children = driver.FindElements(By.TagName("label"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.A:
                {
                    var children = driver.FindElements(By.TagName("a"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Radio:
                {
                    var children = driver.FindElements(By.TagName("radio"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "checkbox"));
                }
                    break;
                case ElementType.Select:
                {
                    var children = driver.FindElements(By.TagName("select"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Span:
                {
                    var children = driver.FindElements(By.TagName("span"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Tbody:
                {
                    var children = driver.FindElements(By.TagName("tbody"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Td:
                {
                    var children = driver.FindElements(By.TagName("td"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Thead:
                {
                    var children = driver.FindElements(By.TagName("thead"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Tr:
                {
                    var children = driver.FindElements(By.TagName("tr"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Table:
                {
                    var children = driver.FindElements(By.TagName("table"));
                    webElements.AddRange(children);
                }
                    break;
                case ElementType.Text:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "text"));
                }
                    break;
                case ElementType.Password:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "password"));
                }
                    break;
                case ElementType.Submit:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "submit"));
                }
                    break;
                case ElementType.DateTime:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "datetime"));
                }
                    break;
                case ElementType.DateTimeLocal:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "datetime-local"));
                }
                    break;
                case ElementType.Date:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "date"));
                }
                    break;
                case ElementType.Color:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "color"));
                }
                    break;
                case ElementType.Email:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "email"));
                }
                    break;
                case ElementType.Month:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "month"));
                }
                    break;
                case ElementType.Number:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "number"));
                }
                    break;
                case ElementType.Range:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "range"));
                }
                    break;
                case ElementType.Search:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "search"));
                }
                    break;
                case ElementType.Tel:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "tel"));
                }
                    break;
                case ElementType.Time:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "time"));
                }
                    break;
                case ElementType.Url:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "url"));
                }
                    break;
                case ElementType.Week:
                {
                    var children = driver.FindElements(By.TagName("input"));
                    webElements.AddRange(children.Where(child => child.GetAttribute("type") == "week"));
                }
                    break;
            }
            return webElements;
        }

        /// <summary>
        ///     Waits until the element is visible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="timespan">The timeout timespan.</param>
        public static void WaitUntilVisible(this IWebElement element, TimeSpan timespan)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var checking = false;
            while (stopwatch.IsRunning && stopwatch.Elapsed < timespan)
            {
                if (!checking)
                {
                    checking = true;
                    if (element.Displayed)
                    {
                        stopwatch.Stop();
                    }
                    else
                    {
                        checking = false;
                    }
                }
            }
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }

        /// <summary>
        ///     Waits until the element exists.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="timespan">The timeout timespan.</param>
        /// <returns></returns>
        public static IWebElement WaitUntilExists(this IWebDriver driver, By locator, TimeSpan timespan)
        {
            IWebElement element = null;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var checking = false;
            while (stopwatch.IsRunning && stopwatch.Elapsed < timespan)
            {
                if (checking) continue;
                checking = true;
                try
                {
                    element = driver.FindElement(locator);
                }
                catch (NotFoundException)
                {
                    checking = false;
                }
                if (element != null)
                {
                    stopwatch.Stop();
                }
                else
                {
                    checking = false;
                }
            }
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
            return element;
        }

        /// <summary>
        ///     Determines whether a alert is present.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <returns>true if a alert is present, otherwise false</returns>
        public static bool IsAlertPresent(this IWebDriver driver)
        {
            try
            {
                var alert = driver.SwitchTo().Alert();
                if (alert != null)
                {
                    driver.SwitchTo().DefaultContent();
                    return true;
                }
                driver.SwitchTo().DefaultContent();
                return false;
            }
            catch
            {
                driver.SwitchTo().DefaultContent();
                return false;
            }
        }

        /// <summary>
        ///     Selects the element by text.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static IWebElement SelectElementByText(this IWebDriver browser, By locator, string text)
        {
            return browser
                .FindElements(locator)
                .SingleOrDefault(d => d.Text == text);
        }

        /// <summary>
        ///     Selects the elements by text.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> SelectElementsByText(this IWebDriver browser, By locator, string text)
        {
            return browser.FindElements(locator)
                .Where(d => d.Text == text);
        }

        /// <summary>
        ///     Waits for ajax to complete.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentException">Web driver must support javascript execution</exception>
        public static void WaitForAjax(this IWebDriver driver)
        {
            while (true)
            {
                bool ajaxIsComplete;
                var javascript = driver as IJavaScriptExecutor;
                if (javascript == null)
                {
                    throw new ArgumentException("Web driver must support javascript execution", nameof(javascript));
                }
                var pageHasJQuery =
                    (bool) javascript.ExecuteScript("if (!window.jQuery) { return false; } else { return true; }");
                if (pageHasJQuery)
                {
                    ajaxIsComplete =
                        (bool)
                            javascript.ExecuteScript(
                                "if (!window.jQuery) { return false; } else { return jQuery.active == 0; }");
                    if (ajaxIsComplete)
                    {
                        break;
                    }
                }
                else
                {
                    ajaxIsComplete = (bool) javascript.ExecuteScript("return document.readyState == 'complete'");
                    if (ajaxIsComplete)
                    {
                        break;
                    }
                }
                Thread.Sleep(100);
            }
        }

        public static IWebElement WaitElement(this IWebDriver driver, By locator)
        {
            WaitForElement(driver, locator, new TimeSpan(0, 0, 30));
            return driver.FindElement(locator);
        }

        public static IWebElement WaitforElement(this IWebDriver driver, By locator, TimeSpan timeSpan)
        {
            WaitForElement(driver, locator, timeSpan);
            return driver.FindElement(locator);
        }

        public static IWebElement WaitElementDisappear(this IWebDriver driver, By locator)
        {
            WaitForElementDisappear(driver, locator, new TimeSpan(0, 0, 30));
            return driver.FindElement(locator);
        }

        public static IWebElement WaitElementDisappear(this IWebDriver driver, By locator, TimeSpan timeSpan)
        {
            WaitForElementDisappear(driver, locator, timeSpan);
            return driver.FindElement(locator);
        }

        public static IWebElement WaitElementIsInvisible(this IWebDriver driver, By locator)
        {
            WaitForElementInvisible(driver, locator, new TimeSpan(0, 0, 30));
            return driver.FindElement(locator);
        }

        public static IWebElement WaitTillElementIsInvisible(this IWebDriver driver, By locator, TimeSpan timeSpan)
        {
            WaitForElementInvisible(driver, locator, timeSpan);
            return driver.FindElement(locator);
        }

        public static IWebElement WaitTillElementIsVisible(this IWebDriver driver, By locator)
        {
            WaitForElementVisible(driver, locator, new TimeSpan(0, 0, 30));
            return driver.FindElement(locator);
        }

        public static IWebElement WaitTillElementIsVisible(this IWebDriver driver, By locator, TimeSpan timeSpan)
        {
            WaitForElementVisible(driver, locator, timeSpan);
            return driver.FindElement(locator);
        }

        private static void WaitForElement(IWebDriver driver, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => d.FindElements(locator).Any());
        }

        private static void WaitForElementDisappear(IWebDriver driver, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => d.FindElements(locator).Any() == false);
        }

        private static void WaitForElementInvisible(IWebDriver driver, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => !d.FindElements(locator).Any(e => e.Displayed));
        }

        private static void WaitForElementVisible(IWebDriver driver, By locator, TimeSpan timespan)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => d.FindElements(locator).Any(e => e.Displayed));
        }

        /// <summary>
        ///     Selects the element by attribute.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IWebElement SelectElementByAttribute(this IWebDriver driver, By locator, string attribute,
            string value)
        {
            return driver.FindElements(locator).SingleOrDefault(d => d.GetAttribute(attribute) == value);
        }

        /// <summary>
        ///     Selects the elements by attribute.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> SelectElementsByAttribute(this IWebDriver driver, By locator,
            string attribute, string value)
        {
            return driver.FindElements(locator)
                .Where(d => d.GetAttribute(attribute) == value);
        }
    }
}