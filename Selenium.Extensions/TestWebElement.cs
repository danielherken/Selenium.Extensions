using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;

namespace Selenium.Extensions
{
    public class TestWebElement: IWebElement, IFindsByLinkText, IFindsById, IFindsByName, IFindsByTagName, IFindsByClassName, IFindsByXPath, IFindsByPartialLinkText, IFindsByCssSelector, IWrapsDriver, IWrapsElement, ILocatable, ITakesScreenshot, ITestWebElement
    {
        private readonly TestSearchContext _context;
        private readonly Func<IWebElement> _elementLookup;
        private readonly Uri _origin;
        private readonly By _selfSelector;
        private readonly TestSettings _settings;
        private IWebElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestWebElement"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        /// <param name="selfSelector">The self selector.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="selfLookup">The self lookup.</param>
        public TestWebElement(TestSettings settings, IWebElement element, By selfSelector, ISearchContext parent, Func<ISearchContext, IWebElement> selfLookup)
        {
            _settings = settings;
            _selfSelector = selfSelector;
            _element = element;
            _origin = DetermineOrigin(_element);
            _elementLookup = () =>
            {
                //has the element become stale and been null:ed by VostokInteractionWrapper
                if (_element == null)
                {
                    //Console.WriteLine("resolving: {0}", selfSelector);
                    _element = selfLookup(parent);

                    var currentUri = DetermineOrigin(_element);

                    if (_origin != null)
                    {
                        if (!MatchesOriginStrictnessLevel(currentUri))
                        {
                            var message =
                                $"Navigation occured between resolving elements. Original element was resolved on {_origin} but a StaleElementReferenceException caused it to be re-resolved on {currentUri}. You can control the sensitivty of this check by changing SamePageOriginStrictness in the settings passed to the VostokWebDriver";
                            throw new InvalidElementStateException(message);
                        }
                    }
                }

                return _element;
            };
            _context = new TestSearchContext(_settings, selfSelector, this, _elementLookup);
        }

        /// <summary>
        /// Finds the element by class name.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns></returns>
        public IWebElement FindElementByClassName(string className)
        {
            return Interact(lmnt => ((IFindsByClassName) lmnt).FindElementByClassName(className));
        }

        /// <summary>
        /// Finds the first element of the specified type.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        public IWebElement FindElementOfType(ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Button:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "button");
                    }
                case ElementType.CheckBox:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Div:
                    {
                        return _element.FindElements(By.TagName("div")).FirstOrDefault();
                    }
                case ElementType.Img:
                    {
                        return _element.FindElements(By.TagName("img")).FirstOrDefault();
                    }
                case ElementType.Label:
                    {
                        return _element.FindElements(By.TagName("label")).FirstOrDefault();
                    }
                case ElementType.A:
                    {
                        return _element.FindElements(By.TagName("a")).FirstOrDefault();
                    }
                case ElementType.Radio:
                    {
                        return
                            _element.FindElements(By.TagName("radio"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Select:
                    {
                        return _element.FindElements(By.TagName("select")).FirstOrDefault();
                    }
                case ElementType.Span:
                    {
                        return _element.FindElements(By.TagName("span")).FirstOrDefault();
                    }
                case ElementType.Tbody:
                    {
                        return _element.FindElements(By.TagName("tbody")).FirstOrDefault();
                    }
                case ElementType.Td:
                    {
                        return _element.FindElements(By.TagName("td")).FirstOrDefault();
                    }
                case ElementType.Thead:
                    {
                        return _element.FindElements(By.TagName("thead")).FirstOrDefault();
                    }
                case ElementType.Tr:
                    {
                        return _element.FindElements(By.TagName("tr")).FirstOrDefault();
                    }
                case ElementType.Table:
                    {
                        return _element.FindElements(By.TagName("table")).FirstOrDefault();
                    }
                case ElementType.Text:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "text");
                    }
                case ElementType.Password:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "password");
                    }
                case ElementType.Submit:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "submit");
                    }
                case ElementType.DateTime:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "datetime");
                    }
                case ElementType.DateTimeLocal:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Date:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "date");
                    }
                case ElementType.Color:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "color");
                    }
                case ElementType.Email:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "email");
                    }
                case ElementType.Month:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "month");
                    }
                case ElementType.Number:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "number");
                    }
                case ElementType.Range:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "range");
                    }
                case ElementType.Search:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "search");
                    }
                case ElementType.Tel:
                    {
                        return
                            _element.FindElements(By.TagName("input"))
                                .FirstOrDefault(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Time:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "time");
                    }
                case ElementType.Url:
                    {
                        return _element.FindElements(By.TagName("input"))
                            .FirstOrDefault(d => d.GetAttribute("type") == "url");
                    }
                case ElementType.Week:
                    {
                        return
                            _element.FindElements(By.TagName("input")).FirstOrDefault(d => d.GetAttribute("type") == "week");
                    }
            }
            return null;

        }

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
        {
            return Interact(lmnt => ((IFindsByClassName) lmnt).FindElementsByClassName(className));
        }

        public IWebElement FindElementByCssSelector(string cssSelector)
        {
            return Interact(lmnt => ((IFindsByCssSelector) lmnt).FindElementByCssSelector(cssSelector));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            return Interact(lmnt => ((IFindsByCssSelector) lmnt).FindElementsByCssSelector(cssSelector));
        }

        public IWebElement FindElementById(string id)
        {
            return Interact(lmnt => ((IFindsById) lmnt).FindElementById(id));
        }

        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            return Interact(lmnt => ((IFindsById) lmnt).FindElementsById(id));
        }


        IWebElement IFindsByLinkText.FindElementByLinkText(string linkText)
        {
            return Interact(lmnt =>
            {
                var e = ((IFindsByLinkText) lmnt);
                return e.FindElementByLinkText(linkText);
            });
        }

        ReadOnlyCollection<IWebElement> IFindsByLinkText.FindElementsByLinkText(string linkText)
        {
            return Interact(lmnt =>
            {
                var e = ((IFindsByLinkText) lmnt);
                return e.FindElementsByLinkText(linkText);
            });
        }

        public IWebElement FindElementByName(string name)
        {
            return Interact(lmnt => ((IFindsByName) lmnt).FindElementByName(name));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            return Interact(lmnt => ((IFindsByName) lmnt).FindElementsByName(name));
        }

        public IWebElement FindElementByPartialLinkText(string partialLinkText)
        {
            return Interact(lmnt => ((IFindsByPartialLinkText) lmnt).FindElementByPartialLinkText(partialLinkText));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
        {
            return Interact(lmnt => ((IFindsByPartialLinkText) lmnt).FindElementsByPartialLinkText(partialLinkText));
        }

        public IWebElement FindElementByTagName(string tagName)
        {
            return Interact(lmnt => ((IFindsByTagName) lmnt).FindElementByTagName(tagName));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            return Interact(lmnt => ((IFindsByTagName) lmnt).FindElementsByTagName(tagName));
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            return Interact(lmnt => ((IFindsByXPath) lmnt).FindElementByXPath(xpath));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            return Interact(lmnt => ((IFindsByXPath) lmnt).FindElementsByXPath(xpath));
        }

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                return Interact(lmnt =>
                {
                    var locatable = (ILocatable) lmnt;
                    return locatable.LocationOnScreenOnceScrolledIntoView;
                });
            }
        }

        public ICoordinates Coordinates
        {
            get
            {
                return Interact(lmnt =>
                {
                    var locatable = (ILocatable) lmnt;
                    return locatable.Coordinates;
                });
            }
        }

        public Screenshot GetScreenshot()
        {
            return Interact(lmnt => ((ITakesScreenshot) lmnt).GetScreenshot());
        }

        public IWebElement FindElement(By @by)
        {
            return _context.FindElement(@by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By @by)
        {
            return _context.FindElements(@by);
        }

        public void Clear()
        {
            Interact(lmnt => lmnt.Clear());
        }

        public void SendKeys(string text)
        {
            Interact(lmnt => lmnt.SendKeys(text));
        }

        public void Submit()
        {
            Interact(lmnt => lmnt.Submit());
        }

        public IWebDriver GetDriver()
        {
            var wrappedElement = _element as IWrapsDriver;
            if (wrappedElement == null)
            {
                var fieldInfo = _element.GetType()
                    .GetField("underlyingElement", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    wrappedElement = fieldInfo.GetValue(_element) as IWrapsDriver;
                    if (wrappedElement == null)
                        throw new ArgumentException("Element must wrap a web driver", nameof(_element));
                }
            }

            return wrappedElement?.WrappedDriver;
        }

        public IJavaScriptExecutor GetJavascriptExecutor()
        {
            var javascriptExecutor = GetDriver() as IJavaScriptExecutor;
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            return javascriptExecutor;
        }

        public T GetAttributeAsType<T>(string attributeName)
        {
            var value = _element.GetAttribute(attributeName) ?? string.Empty;
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        public void Focus()
        {
            var javascriptExecutor = GetJavascriptExecutor();
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            switch (_element.TagName)
            {
                case "input":
                case "select":
                case "textarea":
                case "a":
                case "iframe":
                case "button":
                    javascriptExecutor.ExecuteScript("arguments[0].focus();", _element);
                    break;
            }
        }

        public void Blur()
        {
            var javascriptExecutor = GetJavascriptExecutor();
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            javascriptExecutor.ExecuteAsyncScript("arguments[0].blur();", _element);
        }

        public void Hover()
        {
            var driver = GetDriver();
            var action = new Actions(driver);
            action.MoveToElement(_element).Perform();
        }

        public void HoverAndClick()
        {
            var driver = GetDriver();
            var action = new Actions(driver);
            action.MoveToElement(_element).Click(_element).Build().Perform();
        }

        public void ScrollIntoView()
        {
            var javascriptExecutor = GetJavascriptExecutor();
            if (javascriptExecutor == null)
            {
                throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                    nameof(javascriptExecutor));
            }
            javascriptExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", _element);
        }

        public string ClassName()
        {
            try
            {
                return _element.GetAttribute("class");
            }
            catch
            {
                return null;
            }
        }

        public string Name()
        {
            try
            {
                return _element.GetAttribute("name");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Id()
        {
            try
            {
                return _element.GetAttribute("id");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Style()
        {
            try
            {
                return _element.GetAttribute("style");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Value()
        {
            try
            {
                return _element.GetAttribute("value");
            }
            catch
            {
                return null;
            }
        }

        public string TypeValue()
        {
            try
            {
                return _element.GetAttribute("type");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Title()
        {
            try
            {
                return _element.GetAttribute("title");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IWebElement GetParent()
        {
            return _element.FindElement(By.XPath("./parent::*"));
        }

        public IWebElement GetChild()
        {
            return _element.FindElement(By.XPath("./child::*"));
        }

        public IWebElement GetPreviousSibling()
        {
            return _element.FindElement(By.XPath("./preceding-sibling::*"));
        }

        public IWebElement GetNextSibling()
        {
            return _element.FindElement(By.XPath("./following-sibling::*"));
        }

        public ReadOnlyCollection<IWebElement> GetElementsOfType(ElementType elementType)
        {
            switch (elementType)
            {
                case ElementType.Button:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "button");
                    }
                case ElementType.CheckBox:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Div:
                    {
                        return _element.FindElements(By.TagName("div"));
                    }
                case ElementType.Img:
                    {
                        return _element.FindElements(By.TagName("img"));
                    }
                case ElementType.Label:
                    {
                        return _element.FindElements(By.TagName("label"));
                    }
                case ElementType.A:
                    {
                        return _element.FindElements(By.TagName("a"));
                    }
                case ElementType.Radio:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("radio"))
                            .Where(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Select:
                    {
                        return _element.FindElements(By.TagName("select"));
                    }
                case ElementType.Span:
                    {
                        return _element.FindElements(By.TagName("span"));
                    }
                case ElementType.Tbody:
                    {
                        return _element.FindElements(By.TagName("tbody"));
                    }
                case ElementType.Td:
                    {
                        return _element.FindElements(By.TagName("td"));
                    }
                case ElementType.Thead:
                    {
                        return _element.FindElements(By.TagName("thead"));
                    }
                case ElementType.Tr:
                    {
                        return _element.FindElements(By.TagName("tr"));
                    }
                case ElementType.Table:
                    {
                        return _element.FindElements(By.TagName("table"));
                    }
                case ElementType.Text:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "text");
                    }
                case ElementType.Password:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "password");
                    }
                case ElementType.Submit:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "submit");
                    }
                case ElementType.DateTime:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "datetime");
                    }
                case ElementType.DateTimeLocal:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "datetime-local");
                    }
                case ElementType.Date:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "date");
                    }
                case ElementType.Color:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "color");
                    }
                case ElementType.Email:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "email");
                    }
                case ElementType.Month:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "month");
                    }
                case ElementType.Number:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "number");
                    }
                case ElementType.Range:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "range");
                    }
                case ElementType.Search:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "search");
                    }
                case ElementType.Tel:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "checkbox");
                    }
                case ElementType.Time:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "time");
                    }
                case ElementType.Url:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "url");
                    }
                case ElementType.Week:
                    {
                        return (ReadOnlyCollection<IWebElement>)_element.FindElements(By.TagName("input"))
                            .Where(d => d.GetAttribute("type") == "week");
                    }
            }
            return null;

        }

        public void DoubleClick()
        {
            var driver = GetDriver();
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.DoubleClick(_element).Build();
            action.Perform();
        }

        public void RightClick()
        {
            var driver = GetDriver();
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.ContextClick(_element).Build();
            action.Perform();
        }

        public void ClickAndHold()
        {
            var driver = GetDriver();
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.ClickAndHold(_element).Build();
            action.Perform();
        }

        public void DragAndDrop(IWebElement targetElement)
        {
            var driver = GetDriver();
            var actionsBuilder = new Actions(driver);
            var action = actionsBuilder.DragAndDrop(_element, targetElement).Build();
            action.Perform();
        }


        public void Click()
        {
            Interact(lmnt => lmnt.Click());
        }

        public string GetAttribute(string attributeName)
        {
            return Interact(lmnt => lmnt.GetAttribute(attributeName));
        }

        public string GetCssValue(string propertyName)
        {
            return Interact(lmnt => lmnt.GetCssValue(propertyName));
        }

        public string TagName
        {
            get { return Interact(lmnt => lmnt.TagName); }
        }

        public string Text
        {
            get { return Interact(lmnt => lmnt.Text); }
        }

        public bool Enabled
        {
            get { return Interact(lmnt => lmnt.Enabled); }
        }

        public bool Selected
        {
            get { return Interact(lmnt => lmnt.Selected); }
        }

        public Point Location
        {
            get { return Interact(lmnt => lmnt.Location); }
        }

        public Size Size
        {
            get { return Interact(lmnt => lmnt.Size); }
        }

        public bool Displayed
        {
            get { return Interact(lmnt => lmnt.Displayed); }
        }

        IWebDriver IWrapsDriver.WrappedDriver
        {
            get
            {
                var driver = _element as IWrapsDriver;

                if (driver == null)
                {
                    var fieldInfo = _element.GetType()
                        .GetField("underlyingElement", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        driver = fieldInfo.GetValue(_element) as IWrapsDriver;
                        if (driver == null)
                            throw new ArgumentException("Element must wrap a web driver", nameof(_element));
                    }
                }

                return driver?.WrappedDriver;
            }
        }

        public IWebElement WrappedElement
        {
            get
            {
                Func<IWebElement, IWebElement> unpack = e => e;
                unpack = e =>
                {
                    var innerWrapper = e as IWrapsElement;
                    if (innerWrapper == null)
                    {
                        return e;
                    }

                    return unpack(innerWrapper.WrappedElement);
                };


                return Interact(lmnt => unpack(lmnt));
            }
        }

        private bool MatchesOriginStrictnessLevel(Uri currentUri)
        {
            switch (_settings.SamePageOriginStrictness)
            {
                case TestSettings.PageOriginStrictness.DontCheckOrigin:
                {
                    return true;
                }
                case TestSettings.PageOriginStrictness.AllowNonMatchingAnchorHashes:
                {
                    var originHash0 = _origin.Scheme + _origin.Authority + _origin.Port + _origin.PathAndQuery;
                    var currentHash0 = currentUri.Scheme + currentUri.Authority + currentUri.Port +
                                       currentUri.PathAndQuery;
                    return originHash0.Equals(currentHash0);
                }

                case TestSettings.PageOriginStrictness.AllowNonMatchingQueryStrings:
                {
                    var originHash1 = _origin.Scheme + _origin.Authority + _origin.Port + _origin.AbsolutePath;
                    var currentHash1 = currentUri.Scheme + currentUri.Authority + currentUri.Port +
                                       currentUri.AbsolutePath;
                    return originHash1.Equals(currentHash1);
                }
                case TestSettings.PageOriginStrictness.ExactMatch:
                {
                    return _origin.ToString().Equals(currentUri.ToString());
                }
            }

            throw new Exception("Unhandled PageOriginStrictness setting: " + _settings);
        }

        private Uri DetermineOrigin(IWebElement lmnt)
        {
            var driver = lmnt as IWrapsDriver;

            if (driver == null)
            {
                return null;
            }

            return new Uri(driver.WrappedDriver.Url);
        }

        private void Interact(Action<IWebElement> query)
        {
            TestInteractionWrapper.Interact(ref _element, _selfSelector, () => _elementLookup(), query);
        }

        private T Interact<T>(Func<IWebElement, T> query)
        {
            return TestInteractionWrapper.Interact(ref _element, _selfSelector, () => _elementLookup(), query);
        }
    }
}