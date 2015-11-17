using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Extensions
{
    public class TestWebDriver : IWebDriver, IJavaScriptExecutor, ITakesScreenshot, IHasInputDevices, IHasCapabilities, IAllowsFileDetection, ITestWebDriver
    {
        private readonly TestSearchContext _context;
        private readonly IWebDriver _driver;

        public TestWebDriver(IWebDriver driver) : this(driver, TestSettings.Default)
        {
        }

        public TestWebDriver(IWebDriver driver, TestSettings settings)
        {
            _driver = driver;
            Settings = settings;
            _context = new TestSearchContext(Settings, null, _driver, null);
        }

        public TestSettings Settings { get; }

        public IFileDetector FileDetector
        {
            get { return ((IAllowsFileDetection) _driver).FileDetector; }
            set { ((IAllowsFileDetection) _driver).FileDetector = value; }
        }

        public ICapabilities Capabilities => ((IHasCapabilities) _driver).Capabilities;

        public IKeyboard Keyboard => ((IHasInputDevices) _driver).Keyboard;

        public IMouse Mouse => ((IHasInputDevices) _driver).Mouse;

        public object ExecuteScript(string script, params object[] args)
        {
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
            return ((ITakesScreenshot) _driver).GetScreenshot();
        }

        public IWebElement FindElement(By @by)
        {
            return _context.FindElement(@by);
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
            return _driver.FindElement(By.TagName("body")).Text;
        }

        public bool HasElement(By locator)
        {
            try
            {
                _driver.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }

        public void WaitForPageToLoad(TimeSpan? timeSpan = null)
        {
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.FromSeconds(30);
            }
            var wait = new WebDriverWait(_driver, (TimeSpan)timeSpan);
            var javascript = _driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("Driver must support javascript execution", nameof(_driver));
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

        public void WaitUntiDisplayed(By locator, TimeSpan timeout)
        {
            new WebDriverWait(_driver, timeout)
                .Until(d => d.FindElement(locator).Enabled
                            && d.FindElement(locator).Displayed
                            && d.FindElement(locator).GetAttribute("aria-disabled") == null
                );
        }

        public void WaitUntilVisible(By locator, TimeSpan timeout)
        {
            (new WebDriverWait(_driver, timeout)).Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public void ScrollDownPage()
        {
            var javascript = _driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("Web driver must support javascript execution", nameof(javascript));
            }
            javascript.ExecuteScript("var body = document.body, html  = document.documentElement; var height = Math.max(body.scrollHeight,body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight); window.scrollBy(0, height)");
        }

        public IWebElement FindElementOfType(ElementType elementType)
        {
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
            return _driver
                .FindElements(locator)
                .SingleOrDefault(d => d.Text == text);
        }

        public ReadOnlyCollection<IWebElement> SelectElementsByText(By locator, string text)
        {
            return (ReadOnlyCollection<IWebElement>) _driver.FindElements(locator)
                .Where(d => d.Text == text);
        }

        public void WaitForAjax()
        {
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

        public IWebElement WaitElement(By locator)
        {
            WaitForElement(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitforElement(By locator, TimeSpan timeSpan)
        {
            WaitForElement(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitElementDisappear(By locator)
        {
            WaitForElementDisappear(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitElementDisappear(By locator, TimeSpan timeSpan)
        {
            WaitForElementDisappear(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitElementIsInvisible(By locator)
        {
            WaitForElementInvisible(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitTillElementIsInvisible(By locator, TimeSpan timeSpan)
        {
            WaitForElementInvisible(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitTillElementIsVisible(By locator)
        {
            WaitForElementVisible(_driver, locator, Settings.TimeoutTimeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement WaitTillElementIsVisible(By locator, TimeSpan timeSpan)
        {
            WaitForElementVisible(_driver, locator, timeSpan);
            return _driver.FindElement(locator);
        }

        public IWebElement SelectElementByAttribute(By locator, string attribute, string value)
        {
            return _driver.FindElements(locator).SingleOrDefault(d => d.GetAttribute(attribute) == value);
        }

        public IReadOnlyCollection<IWebElement> SelectElementsByAttribute(By locator, string attribute, string value)
        {
            return (IReadOnlyCollection<IWebElement>) _driver.FindElements(locator)
                .Where(d => d.GetAttribute(attribute) == value);
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