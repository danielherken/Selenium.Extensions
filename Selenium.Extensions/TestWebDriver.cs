using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using Selenium.Extensions.Exceptions;
using Selenium.Extensions.Interfaces;
using Xunit.Abstractions;
using NoSuchElementException = OpenQA.Selenium.NoSuchElementException;

namespace Selenium.Extensions
{
    public class TestWebDriver : IWebDriver, IJavaScriptExecutor, ITakesScreenshot, IHasInputDevices, IHasCapabilities, IAllowsFileDetection, ITestOutputHelper, ITestWebDriver
    {
        private readonly TestSearchContext _context;
        private readonly IWebDriver _driver;
        private string _mainWindowHandle;
        private IHighlighter _highlighter;
        private ILogger _logger;
        private readonly ITestOutputHelper _testOutputHelper;
        private IDialogManager _dialogManager;


        public TestWebDriver(IWebDriver driver, TestSettings settings, ITestOutputHelper testOutputHelper)
        {
            _driver = driver;
            Settings = settings;
            _context = new TestSearchContext(Settings, null, _driver, null);
            _testOutputHelper = testOutputHelper;
        }

        public TestSettings Settings { get; }

        public IFileDetector FileDetector
        {
            get { return ((IAllowsFileDetection) _driver).FileDetector; }
            set { ((IAllowsFileDetection) _driver).FileDetector = value; }
        }

        private IHighlighter Highlighter => _highlighter ?? (_highlighter = new WebDriverHighlighter(this));

        private ILogger Logger => _logger ?? (_logger = new WebDriverLogger(this, _testOutputHelper));

        private IDialogManager DialogManager => _dialogManager ?? (_dialogManager = new DialogManager());

        public ICapabilities Capabilities => ((IHasCapabilities) _driver).Capabilities;

        public IKeyboard Keyboard => ((IHasInputDevices) _driver).Keyboard;

        public IMouse Mouse => ((IHasInputDevices) _driver).Mouse;

        public IWebElement PreviousElement { get; set; }

        public IWebElement CurrentElement { get; set; }

        public object ExecuteScript(string script, params object[] args)
        {
            //return FuncInvoker.Instance.InvokeMethod<object>(() => {
            //    args = base.ParseScriptArguments(args);
            //    object obj = this.m_remoteWebDriver.ExecuteAsyncScript(script, args);
            //    return base.ValidateScriptReturnValue(obj);
            //});
            var executor = (IJavaScriptExecutor) _driver;
            return executor.ExecuteScript(script, args);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            var executor = (IJavaScriptExecutor) _driver;
            return executor.ExecuteAsyncScript(script, args);
        }

        public Screenshot GetScreenshot()
        {
            LogMessage(LogLevel.Verbose, "Taking a screenshot");
            if (string.IsNullOrEmpty(Settings.TestDirectory) || !Directory.Exists(Settings.TestDirectory))
            {
                throw new TestConfigurationException("You have not set a valid directory to save screenshots in. Please set a valid directory first");
            }
            return ((ITakesScreenshot)_driver).GetScreenshot();
        }

        public void SaveScreenShot()
        {
            LogMessage(LogLevel.Verbose, "Taking a full page screenshot");
            Bitmap screenshot;
            if (Settings.DriverType == WebDriverType.ChromeDriver || Settings.DriverType == WebDriverType.SafariDriver)
            {
                screenshot = ScreenShotExtensions.GetFullScreenShot(this, Settings);
            }
            else
            {
                using (var memStream = new MemoryStream(((ITakesScreenshot)_driver).GetScreenshot().AsByteArray))
                {
                    screenshot = new Bitmap(Image.FromStream(memStream));
                }
            }
            screenshot?.Save(Settings.TestDirectory + "ScreenShots" + "\\" + Settings.BrowserName + "_" + WebDriverManager.ScreenShotCounter++ + ".png", ImageFormat.Png);
        }


        public IWebElement FindElement(By @by)
        {
            var foundElement = _context.FindElement(@by);
            if (CurrentElement != null)
            {
                PreviousElement = CurrentElement;
            }
            CurrentElement = foundElement;
            return foundElement;
        }


        public ReadOnlyCollection<IWebElement> FindElements(By @by)
        {
            return _context.FindElements(@by);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        public void Close()
        {
            _driver.Close();
        }

        public void Quit()
        {
            _driver.Quit();
        }

        public IOptions Manage()
        {
            return _driver.Manage();
        }

        public INavigation Navigate()
        {
            return _driver.Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return _driver.SwitchTo();
        }

        public string GetText()
        {
            LogMessage(LogLevel.Verbose, $"Getting text from: [{By.TagName("body")}]");
            return _driver.FindElement(By.TagName("body")).Text;
        }

        public bool HasElement(By locator)
        {
            LogMessage(LogLevel.Verbose, $"Checking if element exists: [{locator}]");
            try
            {
                _driver.FindElement(locator);
                LogMessage(LogLevel.Verbose, $"Element exists: [{locator}]");
            }
            catch (NoSuchElementException)
            {
                LogMessage(LogLevel.Verbose, $"Element does not exist: [{locator}]");
                return false;
            }
            return true;
        }

        public void WaitForPageToLoad(TimeSpan? timeSpan = null)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for page to load: [{Url}]");
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.FromSeconds(30);
            }
            var wait = new WebDriverWait(_driver, (TimeSpan)timeSpan);
            var javascript = _driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                LogMessage(LogLevel.Basic, "Driver must support javascript execution");
                throw new ArgumentException("Driver must support javascript execution", nameof(_driver));
            }
            wait.Until(d =>
            {
                try
                {
                    var readyState = javascript.ExecuteScript(
                        "if (document.readyState) return document.readyState;").ToString();
                    var response = readyState.ToLower();
                    if (response == "complete")
                    {
                        LogMessage(LogLevel.Verbose, $"Page has finished loading: [{Url}]");
                    }
                    return response == "complete";
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    LogMessage(LogLevel.Basic, "Unable to connect to browser");
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    LogMessage(LogLevel.Basic, "Browser no longer available");
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public void WaitUntiDisplayed(By locator, TimeSpan timeout)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to be displayed: [{locator}]");
            new WebDriverWait(_driver, timeout)
                .Until(d => d.FindElement(locator).Enabled
                            && d.FindElement(locator).Displayed
                            && d.FindElement(locator).GetAttribute("aria-disabled") == null
                );
        }

        public void WaitUntilVisible(By locator, TimeSpan timeout)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to be visible: [{locator}]");
            (new WebDriverWait(_driver, timeout)).Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public void ScrollDownPage()
        {
            LogMessage(LogLevel.Verbose, $"Scrolling down page");
            var javascript = _driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("Web driver must support javascript execution", nameof(javascript));
            }
            javascript.ExecuteScript("var body = document.body, html  = document.documentElement; var height = Math.max(body.scrollHeight,body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight); window.scrollBy(0, height)");
        }

        public IWebElement FindElementOfType(ElementType elementType)
        {
            LogMessage(LogLevel.Verbose, $"Finding element of type: [{elementType}]");
            switch (elementType)
            {
                case ElementType.Button:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "button");
                    }
                case ElementType.CheckBox:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Div:
                    {
                        return _driver.FindElements(By.TagName("div")).FirstOrDefault();
                    }
                case ElementType.Img:
                    {
                        return _driver.FindElements(By.TagName("img")).FirstOrDefault();
                    }
                case ElementType.Label:
                    {
                        return _driver.FindElements(By.TagName("label")).FirstOrDefault();
                    }
                case ElementType.A:
                    {
                        return _driver.FindElements(By.TagName("a")).FirstOrDefault();
                    }
                case ElementType.Radio:
                    {
                        return
                            _driver.FindElements(By.TagName("radio"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Select:
                    {
                        return _driver.FindElements(By.TagName("select")).FirstOrDefault();
                    }
                case ElementType.Span:
                    {
                        return _driver.FindElements(By.TagName("span")).FirstOrDefault();
                    }
                case ElementType.Tbody:
                    {
                        return _driver.FindElements(By.TagName("tbody")).FirstOrDefault();
                    }
                case ElementType.Td:
                    {
                        return _driver.FindElements(By.TagName("td")).FirstOrDefault();
                    }
                case ElementType.Thead:
                    {
                        return _driver.FindElements(By.TagName("thead")).FirstOrDefault();
                    }
                case ElementType.Tr:
                    {
                        return _driver.FindElements(By.TagName("tr")).FirstOrDefault();
                    }
                case ElementType.Table:
                    {
                        return _driver.FindElements(By.TagName("table")).FirstOrDefault();
                    }
                case ElementType.Text:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "text");
                    }
                case ElementType.Password:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "password");
                    }
                case ElementType.Submit:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "submit");
                    }
                case ElementType.DateTime:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "datetime");
                    }
                case ElementType.DateTimeLocal:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Date:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "date");
                    }
                case ElementType.Color:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "color");
                    }
                case ElementType.Email:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "email");
                    }
                case ElementType.Month:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "month");
                    }
                case ElementType.Number:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "number");
                    }
                case ElementType.Range:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "range");
                    }
                case ElementType.Search:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "search");
                    }
                case ElementType.Tel:
                    {
                        return
                            _driver.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Time:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "time");
                    }
                case ElementType.Url:
                    {
                        return _driver.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "url");
                    }
                case ElementType.Week:
                    {
                        return
                            _driver.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "week");
                    }
            }
            return null;
        }

        public ReadOnlyCollection<IWebElement> FindElementsOfType(ElementType elementType)
        {
            LogMessage(LogLevel.Verbose, $"Finding elements of type: [{elementType}]");
            switch (elementType)
            {
                case ElementType.Button:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "button");
                    }
                case ElementType.CheckBox:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Div:
                    {
                        return _driver.FindElements(By.TagName("div"));
                    }
                case ElementType.Img:
                    {
                        return _driver.FindElements(By.TagName("img"));
                    }
                case ElementType.Label:
                    {
                        return _driver.FindElements(By.TagName("label"));
                    }
                case ElementType.A:
                    {
                        return _driver.FindElements(By.TagName("a"));
                    }
                case ElementType.Radio:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("radio"))
                            .Where(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Select:
                    {
                        return _driver.FindElements(By.TagName("select"));
                    }
                case ElementType.Span:
                    {
                        return _driver.FindElements(By.TagName("span"));
                    }
                case ElementType.Tbody:
                    {
                        return _driver.FindElements(By.TagName("tbody"));
                    }
                case ElementType.Td:
                    {
                        return _driver.FindElements(By.TagName("td"));
                    }
                case ElementType.Thead:
                    {
                        return _driver.FindElements(By.TagName("thead"));
                    }
                case ElementType.Tr:
                    {
                        return _driver.FindElements(By.TagName("tr"));
                    }
                case ElementType.Table:
                    {
                        return _driver.FindElements(By.TagName("table"));
                    }
                case ElementType.Text:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "text");
                    }
                case ElementType.Password:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "password");
                    }
                case ElementType.Submit:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "submit");
                    }
                case ElementType.DateTime:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "datetime");
                    }
                case ElementType.DateTimeLocal:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "datetime-local");
                    }
                case ElementType.Date:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "date");
                    }
                case ElementType.Color:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "color");
                    }
                case ElementType.Email:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "email");
                    }
                case ElementType.Month:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "month");
                    }
                case ElementType.Number:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "number");
                    }
                case ElementType.Range:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "range");
                    }
                case ElementType.Search:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "search");
                    }
                case ElementType.Tel:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Time:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "time");
                    }
                case ElementType.Url:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "url");
                    }
                case ElementType.Week:
                    {
                        return (ReadOnlyCollection<IWebElement>)_driver.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "week");
                    }
            }
            return null;
        }

        public bool IsAlertPresent()
        {
            LogMessage(LogLevel.Verbose, "Checking if alert is present");
            try
            {
                var alert = _driver.SwitchTo().Alert();
                if (alert != null)
                {
                    _driver.SwitchTo().DefaultContent();
                    return true;
                }
                _driver.SwitchTo().DefaultContent();
                return false;
            }
            catch
            {
                _driver.SwitchTo().DefaultContent();
                return false;
            }
        }

        public IWebElement SelectElementByText(By locator, string text)
        {
            LogMessage(LogLevel.Verbose, $"Selecting element with the text: [{text}]");
            return _driver
                .FindElements(locator)
                .SingleOrDefault(d => d.Text == text);
        }

        public ReadOnlyCollection<IWebElement> SelectElementsByText(By locator, string text)
        {
            LogMessage(LogLevel.Verbose, $"Selecting elements with the text: [{text}]");
            return (ReadOnlyCollection<IWebElement>) _driver.FindElements(locator)
                .Where(d => d.Text == text);
        }

        public void WaitForAjax()
        {
            LogMessage(LogLevel.Verbose, "Waiting for jQuery to finish");
            while (true)
            {
                bool ajaxIsComplete;
                var javascript = _driver as IJavaScriptExecutor;
                if (javascript == null)
                {
                    throw new ArgumentException("Web driver must support javascript execution", nameof(javascript));
                }
                var pageHasJQuery =
                    (bool)javascript.ExecuteScript("if (!window.jQuery) { return false; } else { return true; }");
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
                    ajaxIsComplete = (bool)javascript.ExecuteScript("return document.readyState == 'complete'");
                    if (ajaxIsComplete)
                    {
                        break;
                    }
                }
                Thread.Sleep(100);
            }
        }

        public void Wait(TimeSpan? timeSpan = null)
        {

            if (timeSpan == null)
            {
                timeSpan = TimeSpan.FromSeconds(1);
            }
            var waitTime = (TimeSpan) timeSpan;
            LogMessage(LogLevel.Verbose, $"Waiting for seconds to pass: [{waitTime.Seconds}]");
            Thread.Sleep(waitTime);
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            //while (stopwatch.Elapsed < waitTime)
            //{
            //}
            //if (stopwatch.IsRunning)
            //{
            //    stopwatch.Stop();
            //}
        }

        public IWebElement WaitforElement(By locator, TimeSpan timeSpan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element: [{locator}]");
            WaitForElement(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitElementDisappear(By locator)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to disappear: [{locator}]");
            WaitForElementDisappear(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitElementDisappear(By locator, TimeSpan timeSpan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to disappear: [{locator}]");
            WaitForElementDisappear(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitElementIsInvisible(By locator)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to be visible: [{locator}]");
            WaitForElementInvisible(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitTillElementIsInvisible(By locator, TimeSpan timeSpan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to be invisible: [{locator}]");
            WaitForElementInvisible(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitTillElementIsVisible(By locator)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to be visible: [{locator}]");
            WaitForElementVisible(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitTillElementIsVisible(By locator, TimeSpan timeSpan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to be visible: [{locator}]");
            WaitForElementVisible(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement SelectElementByAttribute(By locator, string attribute, string value)
        {
            LogMessage(LogLevel.Verbose, $"Selecting element by attribute: [{locator}]");
            return _driver.FindElements(locator).SingleOrDefault(d => d.GetAttribute(attribute) == value);
        }

        public IReadOnlyCollection<IWebElement> SelectElementsByAttribute(By locator, string attribute, string value)
        {
            LogMessage(LogLevel.Verbose, $"Selecting elements by attribute: [{locator}]");
            return (IReadOnlyCollection<IWebElement>) _driver.FindElements(locator)
                .Where(d => d.GetAttribute(attribute) == value);
        }

        public void SwitchToAlert(AlertAction alertAction)
        {
            LogMessage(LogLevel.Verbose, "Switching to alert");
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.IgnoreExceptionTypes(typeof(NoAlertPresentException));
            wait.Until(ExpectedConditions.AlertIsPresent());

            var alert = _driver.SwitchTo().Alert();
            switch (alertAction)
            {
                case AlertAction.Accept:
                    LogMessage(LogLevel.Verbose, "Accepting the alert");
                    alert.Accept();
                    break;
                case AlertAction.Dismiss:
                    LogMessage(LogLevel.Verbose, "Dismissing the alert");
                    alert.Dismiss();
                    break;
            }

        }

        public void SwitchToPrompt(AlertAction alertAction, string promptValue)
        {
            LogMessage(LogLevel.Verbose, "Switching to prompt");
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.IgnoreExceptionTypes(typeof(NoAlertPresentException));
            wait.Until(ExpectedConditions.AlertIsPresent());

            var alert = _driver.SwitchTo().Alert();
            switch (alertAction)
            {
                case AlertAction.Accept:
                    LogMessage(LogLevel.Verbose, $"Accepting the prompt with the value: [{promptValue}]");
                    alert.SendKeys(promptValue);
                    alert.Accept();
                    break;
                case AlertAction.Dismiss:
                    LogMessage(LogLevel.Verbose, "Dismissing the prompt");
                    alert.Dismiss();
                    break;
            }
        }

        public void SwitchToWindow(string windowTitle)
        {
            LogMessage(LogLevel.Verbose, $"Switching to window: [{windowTitle}]");
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.IgnoreExceptionTypes(typeof(NoAlertPresentException));
            wait.Until(driver => _driver.WindowHandles.Count > 1);

            //var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            //wait.IgnoreExceptionTypes(typeof(NoAlertPresentException));
            //wait.Until(ExpectedConditions.);


            var windowHandles = _driver.WindowHandles;
            _mainWindowHandle = _driver.CurrentWindowHandle;
            foreach (string handle in windowHandles.Where(handle => handle != _mainWindowHandle))
            {
                if (_driver.SwitchTo().Window(handle).Title != windowTitle)
                {
                    _driver.SwitchTo().Window(_mainWindowHandle);
                }
                else
                {
                    break;
                }
            }
        }

        public void ClosePopUpWindow()
        {
            LogMessage(LogLevel.Verbose, $"Closing popup window");
            _driver.Close();
            if (!string.IsNullOrEmpty(_mainWindowHandle))
            {
                _driver.SwitchTo().Window(_mainWindowHandle);
            }
            else
            {
                _driver.SwitchTo().DefaultContent();
            }
            
        }


        public void Highlight(By locator, bool removeHighlight = false)
        {
            string highlightLogText = removeHighlight ? "Un-Highlight" : "Highlight";
            //AppendVerbose(string.Format("{0} locator: [{1}]", highlightLogText, locator));

            object element = _driver.FindElement(locator);
            Highlighter.HighlightElement(element, removeHighlight);
        }

        public void Highlight(IWebElement webElement, bool removeHighlight = false)
        {
            Highlighter.HighlightElement(webElement, removeHighlight);
        }

        public void LogMessage(LogLevel level, string message)
        {
            Logger.AppendLogEntry(level, message);
        }

        public bool UsingjQuery()
        {
            var javascript = _driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                LogMessage(LogLevel.Basic, "Driver must support javascript execution");
                throw new ArgumentException("Driver must support javascript execution", nameof(_driver));
            }
            bool result = false;
            try
            {
                result = (bool)javascript.ExecuteScript("return typeof jQuery == 'function'");
            }
            catch (WebDriverException)
            {
            }
            return result;
        }

        public Size GetViewport()
        {
            var javascriptExecutor = _driver as IJavaScriptExecutor;
            if (javascriptExecutor == null)
            {
                LogMessage(LogLevel.Basic, "Driver must support javascript execution");
                throw new ArgumentException("Driver must support javascript execution", nameof(_driver));
            }
            const string Javascript =
    @"{var a;var b;if(typeof window.innerWidth!=""undefined""){a=window.innerWidth,b=window.innerHeight}else if(typeof document.documentElement!=""undefined""&&typeof document.documentElement.clientWidth!=""undefined""&&document.documentElement.clientWidth!=0){a=document.documentElement.clientWidth,b=document.documentElement.clientHeight}else{a=document.getElementsByTagName(""body"")[0].clientWidth,b=document.getElementsByTagName(""body"")[0].clientHeight}return[a,b]}";
            var dimensions = (ReadOnlyCollection<object>)javascriptExecutor.ExecuteScript(Javascript);
            return new Size(Convert.ToInt32(dimensions[0]), Convert.ToInt32(dimensions[1]));
        }

        public void SelectFile(string directoryName, string[] files)
        {
            DialogManager.SelectFiles(Settings.DriverType, directoryName, files);
        }

        public void SwitchToIframe(int index, By childLocator)
        {
            if (childLocator != null)
            {
                var childElement = _driver.FindElement(childLocator) as ITestWebElement;
                var iframe = childElement?.GetParentOfType("iframe") as ITestWebElement;
                if (iframe != null)
                {
                    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                    wait.IgnoreExceptionTypes(typeof(NoAlertPresentException));
                    wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(iframe.FoundBy));
                }
            }

            _driver.SwitchTo().Frame(index);
        }

        public string Url
        {
            get { return _driver.Url; }
            set { _driver.Url = value; }
        }

        public string Title => _driver.Title;

        public string PageSource => _driver.PageSource;

        public string CurrentWindowHandle => _driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles
        {
            get { return new EagerReadOnlyCollection<string>(() => _driver.WindowHandles); }
        }

        private void WaitForElement(IWebDriver driver, By locator, TimeSpan timespan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element: [{locator}]");
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => d.FindElements(locator).Any());
        }

        private void WaitForElementDisappear(IWebDriver driver, By locator, TimeSpan timespan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to disappear: [{locator}]");
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => d.FindElements(locator).Any() == false);
        }

        private void WaitForElementInvisible(IWebDriver driver, By locator, TimeSpan timespan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to to be invisible: [{locator}]");
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => !d.FindElements(locator).Any(e => e.Displayed));
        }

        private void WaitForElementVisible(IWebDriver driver, By locator, TimeSpan timespan)
        {
            LogMessage(LogLevel.Verbose, $"Waiting for element to to be visible: [{locator}]");
            IWait<IWebDriver> wait = new WebDriverWait(driver, timespan);
            wait.Until(d => d.FindElements(locator).Any(e => e.Displayed));
        }

        public void WriteLine(string message)
        {
            _testOutputHelper.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _testOutputHelper.WriteLine(format, args);
        }

    }

    public class EagerReadOnlyCollection<T>
        : ReadOnlyCollection<T>
    {
        public EagerReadOnlyCollection(Func<IEnumerable<T>> collection)
            : this(new EagerList<T>(collection))
        {
        }

        public EagerReadOnlyCollection(EagerList<T> list)
            : base(list)
        {
        }
    }

    public class EagerList<T> : IList<T>
    {
        private readonly Func<IEnumerable<T>> _collectionQuery;

        public EagerList(Func<IEnumerable<T>> collection)
        {
            _collectionQuery = collection;
        }

        private IEnumerable<T> Collection => _collectionQuery();

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new InvalidOperationException("Cannot add items to a read only collection.");
        }

        public void Clear()
        {
            throw new InvalidOperationException("Cannot clear a read only collection.");
        }

        public bool Contains(T item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException("Cannot remove items from a read only collection.");
        }

        public int Count => Collection.Count();

        public bool IsReadOnly => true;

        public int IndexOf(T item)
        {
            return Collection.ToList().IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException("Cannot insert items in a read only collection.");
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException("Cannot remove items from a read only collection.");
        }

        public T this[int index]
        {
            get { return Collection.ToList()[index]; }
            set { throw new InvalidOperationException("Cannot modify items in a read only collection."); }
        }
    }
}