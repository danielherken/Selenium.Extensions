using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;

namespace Selenium.Extensions
{
    public static class ElementExtensions
    {
        /// <summary>
        ///     Gets the driver.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IWebDriver GetDriver(this IWebElement element)
        {
            var wrappedElement = element as IWrapsDriver;
            if (wrappedElement == null)
            {
                var fieldInfo = element.GetType()
                    .GetField("underlyingElement", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    wrappedElement = fieldInfo.GetValue(element) as IWrapsDriver;
                    if (wrappedElement == null)
                        throw new ArgumentException("Element must wrap a web driver", nameof(element));
                }
            }

            return wrappedElement?.WrappedDriver;
        }


        /// <summary>
        ///     Gets the javascript executor.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IJavaScriptExecutor GetJavascriptExecutor(this IWebElement element)
        {
            var driver = GetDriver(element);
            var javascriptExecutor = driver as IJavaScriptExecutor;
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            return javascriptExecutor;
        }

        /// <summary>
        ///     Gets the type of the element attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static T GetAttributeAsType<T>(this IWebElement element, string attributeName)
        {
            var value = element.GetAttribute(attributeName) ?? string.Empty;
            return (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFromString(value);
        }

        /// <summary>
        ///     Focuses on the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void Focus(this IWebElement element)
        {
            var javascriptExecutor = GetJavascriptExecutor(element);
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            switch (element.TagName)
            {
                case "input":
                case "select":
                case "textarea":
                case "a":
                case "iframe":
                case "button":
                    javascriptExecutor.ExecuteScript("arguments[0].focus();", element);
                    break;
            }
        }

        /// <summary>
        ///     Blurs over specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void Blur(this IWebElement element)
        {
            var javascriptExecutor = GetJavascriptExecutor(element);
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            javascriptExecutor.ExecuteAsyncScript("arguments[0].blur();", element);
        }

        /// <summary>
        ///     Hovers over specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void Hover(this IWebElement element)
        {
            var driver = GetDriver(element);
            var action = new Actions(driver);
            action.MoveToElement(element).Perform();
        }

        /// <summary>
        ///     Hovers over and then clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void HoverAndClick(this IWebElement element)
        {
            var driver = GetDriver(element);
            var action = new Actions(driver);
            action.MoveToElement(element).Click(element).Build().Perform();
        }

        /// <summary>
        ///     Determines if the element is displayed.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>true if the element is displayed, otherwise false</returns>
        public static bool Displayed(this IWebElement element)
        {
            try
            {
                return element.Displayed;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Scrolls the element into view.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void ScrollIntoView(this IWebElement element)
        {
            var javascriptExecutor = GetJavascriptExecutor(element);
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            javascriptExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        /// <summary>
        ///     Gets the specified elements class attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The class attribute value</returns>
        public static string ClassName(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("class");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements name attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The name attribute value</returns>
        public static string GetName(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("name");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements id attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The id attribute value</returns>
        public static string GetId(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("id");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements style attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The style attribute value</returns>
        public static string GetStyle(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("style");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements value attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value attribute</returns>
        public static string GetValue(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("value");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements type attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The type attribute value</returns>
        public static string GetTypeValue(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("type");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements title attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The title attribute value</returns>
        public static string GetTitle(this IWebElement element)
        {
            try
            {
                return element.GetAttribute("title");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets the specified elements parent element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The parent element</returns>
        public static IWebElement GetParent(this IWebElement element)
        {
            return element.FindElement(By.XPath("./parent::*"));
        }

        /// <summary>
        ///     Gets the specified elements child element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The child element</returns>
        public static IWebElement GetChild(this IWebElement element)
        {
            return element.FindElement(By.XPath("./child::*"));
        }

        /// <summary>
        ///     Gets the preceding elements sibling.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IWebElement GetPreviousSibling(this IWebElement element)
        {
            return element.FindElement(By.XPath("./preceding-sibling::*"));
        }

        /// <summary>
        ///     Gets the following elements sibling.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IWebElement GetNextSibling(this IWebElement element)
        {
            return element.FindElement(By.XPath("./following-sibling::*"));
        }

        public static IWebElement FindElementOfType(this IWebElement element, ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Button:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "button");
                }
                case ElementType.CheckBox:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Div:
                {
                    return element.FindElements(By.TagName("div")).FirstOrDefault();
                }
                case ElementType.Img:
                {
                    return element.FindElements(By.TagName("img")).FirstOrDefault();
                }
                case ElementType.Label:
                {
                    return element.FindElements(By.TagName("label")).FirstOrDefault();
                }
                case ElementType.A:
                {
                    return element.FindElements(By.TagName("a")).FirstOrDefault();
                }
                case ElementType.Radio:
                {
                    return
                        element.FindElements(By.TagName("radio"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Select:
                {
                    return element.FindElements(By.TagName("select")).FirstOrDefault();
                }
                case ElementType.Span:
                {
                    return element.FindElements(By.TagName("span")).FirstOrDefault();
                }
                case ElementType.Tbody:
                {
                    return element.FindElements(By.TagName("tbody")).FirstOrDefault();
                }
                case ElementType.Td:
                {
                    return element.FindElements(By.TagName("td")).FirstOrDefault();
                }
                case ElementType.Thead:
                {
                    return element.FindElements(By.TagName("thead")).FirstOrDefault();
                }
                case ElementType.Tr:
                {
                    return element.FindElements(By.TagName("tr")).FirstOrDefault();
                }
                case ElementType.Table:
                {
                    return element.FindElements(By.TagName("table")).FirstOrDefault();
                }
                case ElementType.Text:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "text");
                }
                case ElementType.Password:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "password");
                }
                case ElementType.Submit:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "submit");
                }
                case ElementType.DateTime:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "datetime");
                }
                case ElementType.DateTimeLocal:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Date:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "date");
                }
                case ElementType.Color:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "color");
                }
                case ElementType.Email:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "email");
                }
                case ElementType.Month:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "month");
                }
                case ElementType.Number:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "number");
                }
                case ElementType.Range:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "range");
                }
                case ElementType.Search:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "search");
                }
                case ElementType.Tel:
                {
                    return
                        element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Time:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "time");
                }
                case ElementType.Url:
                {
                    return element.FindElements(By.TagName("input"))
                        .FirstOrDefault(d => d.GetAttribute("type") == "url");
                }
                case ElementType.Week:
                {
                    return
                        element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "week");
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the children elements of the specified type.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetElementsOfType(this IWebElement element,
            ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Button:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "button");
                }
                case ElementType.CheckBox:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Div:
                {
                    return element.FindElements(By.TagName("div"));
                }
                case ElementType.Img:
                {
                    return element.FindElements(By.TagName("img"));
                }
                case ElementType.Label:
                {
                    return element.FindElements(By.TagName("label"));
                }
                case ElementType.A:
                {
                    return element.FindElements(By.TagName("a"));
                }
                case ElementType.Radio:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("radio"))
                        .Where(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Select:
                {
                    return element.FindElements(By.TagName("select"));
                }
                case ElementType.Span:
                {
                    return element.FindElements(By.TagName("span"));
                }
                case ElementType.Tbody:
                {
                    return element.FindElements(By.TagName("tbody"));
                }
                case ElementType.Td:
                {
                    return element.FindElements(By.TagName("td"));
                }
                case ElementType.Thead:
                {
                    return element.FindElements(By.TagName("thead"));
                }
                case ElementType.Tr:
                {
                    return element.FindElements(By.TagName("tr"));
                }
                case ElementType.Table:
                {
                    return element.FindElements(By.TagName("table"));
                }
                case ElementType.Text:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "text");
                }
                case ElementType.Password:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "password");
                }
                case ElementType.Submit:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "submit");
                }
                case ElementType.DateTime:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "datetime");
                }
                case ElementType.DateTimeLocal:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "datetime-local");
                }
                case ElementType.Date:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "date");
                }
                case ElementType.Color:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "color");
                }
                case ElementType.Email:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "email");
                }
                case ElementType.Month:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "month");
                }
                case ElementType.Number:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "number");
                }
                case ElementType.Range:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "range");
                }
                case ElementType.Search:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "search");
                }
                case ElementType.Tel:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "checkbox");
                }
                case ElementType.Time:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "time");
                }
                case ElementType.Url:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "url");
                }
                case ElementType.Week:
                {
                    return (ReadOnlyCollection<IWebElement>) element.FindElements(By.TagName("input"))
                        .Where(d => d.GetAttribute("type") == "week");
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the children elements by.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetElementsBy(this IWebElement element, By locator)
        {
            return element.FindElements(locator);
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
        /// <param name="element">The element.</param>
        /// <param name="timespan">The timeout timespan.</param>
        public static void WaitUntilExists(this IWebElement element, TimeSpan timespan)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var checking = false;
            while (stopwatch.IsRunning && stopwatch.Elapsed < timespan)
            {
                if (!checking)
                {
                    checking = true;
                    if (element != null)
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
        ///     Double clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void DoubleClick(this IWebElement element)
        {
            var driver = GetDriver(element);
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.DoubleClick(element).Build();
            action.Perform();
        }

        /// <summary>
        ///     Right clicks the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void RightClick(this IWebElement element)
        {
            var driver = GetDriver(element);
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.ContextClick(element).Build();
            action.Perform();
        }

        /// <summary>
        ///     Clicks and hold the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void ClickAndHold(this IWebElement element)
        {
            var driver = GetDriver(element);
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.ClickAndHold(element).Build();
            action.Perform();
        }

        /// <summary>
        ///     Drags and drops a element onto another element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="targetElement">The target element.</param>
        public static void DragAndDrop(this IWebElement element, IWebElement targetElement)
        {
            var driver = GetDriver(element);
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.DragAndDrop(element, targetElement).Build();
            action.Perform();
        }

        public static bool IsCheckboxOrRadio(this IWebElement element)
        {
            if (!element.TagIs("input"))
            {
                return false;
            }
            const string attribute = "type";
            var att = element.GetAttribute(attribute);
            if (string.IsNullOrWhiteSpace(att))
            {
                return false;
            }
            var result =
                att.Equals("checkbox", StringComparison.OrdinalIgnoreCase) ||
                att.Equals("radio", StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsChecked(this IWebElement element)
        {
            if (!element.TagIs("input"))
            {
                return false;
            }
            var attribute = "checked";
            var att = element.GetAttribute(attribute);
            if (string.IsNullOrWhiteSpace(att))
            {
                return false;
            }
            var result =
                att.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                att.Equals("checked", StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsCheckbox(this IWebElement element)
        {
            if (!element.TagIs("input"))
            {
                return false;
            }
            const string attribute = "type";
            var att = element.GetAttribute(attribute);
            if (string.IsNullOrWhiteSpace(att))
            {
                return false;
            }
            var result = att.Equals("checkbox", StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsRadio(this IWebElement element)
        {
            if (!element.TagIs("input"))
            {
                return false;
            }
            var attribute = "type";
            var att = element.GetAttribute(attribute);
            var result = att.Equals("radio", StringComparison.OrdinalIgnoreCase);
            return result;
        }


        public static bool IsTextInput(this IWebElement element)
        {
            if (!element.TagIs("input"))
            {
                return false;
            }
            var att = element.GetAttribute("type");
            var result = att.Equals("text", StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsTextArea(this IWebElement element)
        {
            var result = element.TagIs("textarea");
            return result;
        }

        public static bool Contains(this IWebElement element, string value)
        {
            if (string.IsNullOrWhiteSpace(element.Text))
            {
                return false;
            }
            var result = element.Text.Contains(value);
            return result;
        }

        public static bool IsSelected(this IWebElement element)
        {
            if (!IsOptionTag(element))
            {
                return false;
            }
            var att = element.GetAttribute("selected");
            if (string.IsNullOrWhiteSpace(att))
            {
                return false;
            }
            var result =
                att.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                att.Equals("selected", StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static bool IsOptionTag(this IWebElement element)
        {
            var result = element.TagIs("option");
            return result;
        }

        public static bool TagIs(this IWebElement element, string tag)
        {
            var result = element.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public static IWebElement ClearText(this IWebElement element)
        {
            element.Clear();
            return element;
        }

        public static void SetTextForControl(this IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
        }
    }
}