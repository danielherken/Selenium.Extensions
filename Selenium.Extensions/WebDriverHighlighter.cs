using System;
using System.Diagnostics;
using OpenQA.Selenium;
using Selenium.Extensions.Interfaces;

namespace Selenium.Extensions
{
    internal class WebDriverHighlighter : IHighlighter
    {
        /// <summary>
        ///     The core
        /// </summary>
        private readonly ITestWebDriver _driver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebDriverHighlighter" /> class.
        /// </summary>
        /// <param name="driver">
        ///     The driver.
        /// </param>
        public WebDriverHighlighter(ITestWebDriver driver)
        {
            _driver = driver;
        }

        public void Highlight(By locator, bool reset)
        {
            IWebElement element;
            try
            {
                element = _driver.FindElement(locator);
            }
            catch (Exception)
            {
                return;
            }

            HighlightElement(element, reset);
        }

        public void HighlightElement(object element, bool reset = false)
        {
            if (element == null)
            {
                return;
            }
            if (!_driver.Settings.HighlightElements) return;
            bool internetExplorer = _driver.Settings.DriverType == WebDriverType.InternetExplorerDriver;
            try
            {
                IJavaScriptExecutor javaScriptExecutor = _driver as IJavaScriptExecutor;
                if (javaScriptExecutor != null)
                {
                    string elementType = (string)javaScriptExecutor.ExecuteScript("return arguments[0].getAttribute('type');", element);
                    bool doWrap = elementType == "radio" || elementType == "checkbox";

                    if (!reset)
                    {
                        var highlightCss = "background-color:" + _driver.Settings.HighlightingStyle.BackgroundColor + "; border:" + _driver.Settings.HighlightingStyle.BorderSizeInPixels + "px " + _driver.Settings.HighlightingStyle.BorderStyle.ToString().ToLowerInvariant() + " " + _driver.Settings.HighlightingStyle.BorderColor + ";";
                        // Wrap the element in a highlighted span
                        if (doWrap)
                        {
                            if (internetExplorer)
                            {
                                javaScriptExecutor.ExecuteScript(
                                    @"{var wrapper=document.createElement('span'); " +
                                    $@"wrapper.style.cssText='{highlightCss}';" +
                                    @"wrapper.id='webinatorHighlightWrapper'; " +
                                    @"arguments[0].parentNode.insertBefore(wrapper, arguments[0]);" +
                                    @"wrapper.appendChild(arguments[0]);} ",
                                    element);
                            }
                            else
                            {
                                javaScriptExecutor.ExecuteScript(
                                    @"{var wrapper=document.createElement('span'); " +
                                    $@"wrapper.setAttribute('style','{highlightCss}');" +
                                    @"wrapper.id='webinatorHighlightWrapper'; " +
                                    @"arguments[0].parentNode.insertBefore(wrapper, arguments[0]);" +
                                    @"wrapper.appendChild(arguments[0]);} ",
                                    element);
                            }
                        }
                        else
                        {
                            // Get the current styling
                            string stylePrevious = (string)javaScriptExecutor.ExecuteScript(
                                internetExplorer
                                    ? "return arguments[0].style.cssText;"
                                    : "return arguments[0].getAttribute('style');",
                                element) ?? string.Empty;

                            // Store it in a custom attribute
                            javaScriptExecutor.ExecuteScript(
                                $"arguments[0].setAttribute('webinatorOldStyle','{stylePrevious}');", element);

                            // Set the new style
                            javaScriptExecutor.ExecuteScript(
                                internetExplorer
                                    ? $"arguments[0].style.cssText='{highlightCss}';"
                                    : $"arguments[0].setAttribute('style','{highlightCss}');",
                                element);

                            // Reset the old style after timeout
                            //((IJavaScriptExecutor)_core.GetCore()).ExecuteScript(
                            //    internetExplorer
                            //    ? string.Format(@"window.setTimeout(function() {{ arguments[0].style.cssText='{0}';  }}, 1000);", stylePrevious)
                            //    : string.Format(@"window.setTimeout(function() {{ arguments[0].setAttribute('style','{0}');  }}, 1000);", stylePrevious),
                            //    element);
                        }
                    }
                    else
                    {
                        // Unwrap the element to remove the higlight
                        if (doWrap)
                        {
                            javaScriptExecutor.ExecuteScript(
                                @"{var original=document.getElementById('webinatorHighlightWrapper');" +
                                @"var child = original.firstChild;" +
                                @"var parent = original.parentNode;" +
                                @"parent.insertBefore(child, original);" +
                                @"parent.removeChild(original);}");
                        }
                        else
                        {
                            // Get the previous style
                            var stylePrevious = (string)javaScriptExecutor.ExecuteScript("return arguments[0].getAttribute('webinatorOldStyle');", element);

                            // Remove the custom attribute
                            javaScriptExecutor.ExecuteScript("arguments[0].removeAttribute('webinatorOldStyle');", element);

                            // Set the style to the previous style, or remove the style attribute entirely
                            if (string.IsNullOrEmpty(stylePrevious))
                            {
                                javaScriptExecutor.ExecuteScript("arguments[0].removeAttribute('style');", element);
                            }
                            else
                            {
                                javaScriptExecutor.ExecuteScript(
                                    internetExplorer
                                        ? $"arguments[0].style.cssText='{stylePrevious}'"
                                        : $"arguments[0].setAttribute('style','{stylePrevious}');",
                                    element);
                            }
                        }
                    }
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch(Exception ex)
// ReSharper restore EmptyGeneralCatchClause
            {
                Debug.WriteLine(ex.Message);
                // We don't really care if the highlight operation failed, so let's just move on
            }
        }
    }
}