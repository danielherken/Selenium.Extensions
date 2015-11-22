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
using OpenQA.Selenium.Support.UI;
using Selenium.Extensions.Interfaces;

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
        private IJavaScriptExecutor _javaScriptExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestWebElement" /> class.
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
                //Has the element become stale and been nulled
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
                                $"Navigation occured between resolving elements. Original element was resolved on {_origin} but a StaleElementReferenceException caused it to be re-resolved on {currentUri}. You can control the sensitivty of this check by changing SamePageOriginStrictness in the settings passed to the TestWebDriver";
                            throw new InvalidElementStateException(message);
                        }
                    }
                }

                return _element;
            };
            _context = new TestSearchContext(_settings, selfSelector, this, _elementLookup);
        }

        public By FoundBy => _selfSelector;

        /// <summary>
        /// Finds the element by class name.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns></returns>
        public IWebElement FindElementByClassName(string className)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by class name [{className}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByClassName) elmnt).FindElementByClassName(className));
        }

        public IJavaScriptExecutor JavaScriptExecutor
        {
            get
            {
                if (_javaScriptExecutor == null)
                {
                    _javaScriptExecutor = GetJavascriptExecutor();
                    if (_javaScriptExecutor == null)
                    {
                        throw new ArgumentException("Element must wrap a web driver that supports javascript execution",
                            nameof(_javaScriptExecutor));
                    }
                }
                return _javaScriptExecutor;
            }
            set
            {
                _javaScriptExecutor = value;
            }
        }

        /// <summary>
        /// Finds the first element of the specified type.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        public IWebElement FindElementOfType(ElementType elementType)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element of type [{elementType}] from " + _selfSelector);
            switch (elementType)
            {
                case ElementType.Div:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("div").FirstOrDefault());
                    }
                case ElementType.Img:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("img").FirstOrDefault());
                    }
                case ElementType.Label:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("label").FirstOrDefault());
                    }
                case ElementType.A:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("a").FirstOrDefault());
                    }
                case ElementType.Select:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("select").FirstOrDefault());
                    }
                case ElementType.Span:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("span").FirstOrDefault());
                    }
                case ElementType.Tbody:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("tbody").FirstOrDefault());
                    }
                case ElementType.Td:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("td").FirstOrDefault());
                    }
                case ElementType.Thead:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("thead").FirstOrDefault());
                    }
                case ElementType.Tr:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("tr").FirstOrDefault());
                    }
                case ElementType.Table:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("table").FirstOrDefault());
                    }
                case ElementType.Radio:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("radio").FirstOrDefault(d => d.GetAttribute("type") == "checkbox"));
                    }
                case ElementType.Button:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "button"));
                    }
                case ElementType.CheckBox:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "checkbox"));
                    }
                case ElementType.Text:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "text"));
                    }
                case ElementType.Password:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "password"));
                    }
                case ElementType.Submit:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "submit"));
                    }
                case ElementType.DateTime:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "datetime"));
                    }
                case ElementType.DateTimeLocal:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "datetime-local"));
                    }
                case ElementType.Date:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "date"));
                    }
                case ElementType.Color:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "color"));
                    }
                case ElementType.Email:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "email"));
                    }
                case ElementType.Month:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "month"));
                    }
                case ElementType.Number:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "number"));
                    }
                case ElementType.Range:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "range"));
                    }
                case ElementType.Search:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "search"));
                    }
                case ElementType.Tel:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "tel"));
                    }
                case ElementType.Time:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "time"));
                    }
                case ElementType.Url:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "url"));
                    }
                case ElementType.Week:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").FirstOrDefault(d => d.GetAttribute("type") == "week"));
                    }
            }
            return null;

        }

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by class name [{className}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByClassName) elmnt).FindElementsByClassName(className));
        }

        public IWebElement FindElementByCssSelector(string cssSelector)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by css selector [{cssSelector}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByCssSelector) elmnt).FindElementByCssSelector(cssSelector));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by css selector [{cssSelector}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByCssSelector) elmnt).FindElementsByCssSelector(cssSelector));
        }

        public IWebElement FindElementById(string id)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by id [{id}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsById) elmnt).FindElementById(id));
        }

        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by id [{id}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsById) elmnt).FindElementsById(id));
        }


        IWebElement IFindsByLinkText.FindElementByLinkText(string linkText)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by link text [{linkText}] from " + _selfSelector);
            return Interact(elmnt =>
            {
                var e = ((IFindsByLinkText) elmnt);
                return e.FindElementByLinkText(linkText);
            });
        }

        ReadOnlyCollection<IWebElement> IFindsByLinkText.FindElementsByLinkText(string linkText)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by link text [{linkText}] from " + _selfSelector);
            return Interact(elmnt =>
            {
                var e = ((IFindsByLinkText) elmnt);
                return e.FindElementsByLinkText(linkText);
            });
        }

        public IWebElement FindElementByName(string name)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by name [{name}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByName) elmnt).FindElementByName(name));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by name [{name}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByName) elmnt).FindElementsByName(name));
        }

        public IWebElement FindElementByPartialLinkText(string partialLinkText)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element partial link text [{partialLinkText}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByPartialLinkText) elmnt).FindElementByPartialLinkText(partialLinkText));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements partial link text [{partialLinkText}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByPartialLinkText) elmnt).FindElementsByPartialLinkText(partialLinkText));
        }

        public IWebElement FindElementByTagName(string tagName)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by tag name [{tagName}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByTagName) elmnt).FindElementByTagName(tagName));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by tag name [{tagName}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByTagName) elmnt).FindElementsByTagName(tagName));
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by xpath [{xpath}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByXPath) elmnt).FindElementByXPath(xpath));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by xpath [{xpath}] from " + _selfSelector);
            return Interact(elmnt => ((IFindsByXPath) elmnt).FindElementsByXPath(xpath));
        }

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting scroll position");
                return Interact(elmnt =>
                {
                    var locatable = (ILocatable) elmnt;
                    return locatable.LocationOnScreenOnceScrolledIntoView;
                });
            }
        }

        public ICoordinates Coordinates
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting screen coordinates");
                return Interact(elmnt =>
                {
                    var locatable = (ILocatable) elmnt;
                    return locatable.Coordinates;
                });
            }
        }

        public Screenshot GetScreenshot()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting a creenshot");
            return Interact(elmnt => ((ITakesScreenshot) elmnt).GetScreenshot());
        }

        public IWebElement FindElement(By @by)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding element by: [{@by}] from " + _selfSelector);
            return _context.FindElement(@by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By @by)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Finding elements by: [{@by}] from " + _selfSelector);
            return _context.FindElements(@by);
        }


        public void Clear()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Clearing element for " + _selfSelector);
            Interact(elmnt => elmnt.Clear());
        }

        public void SendKeys(string text)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Sending keys: [{text}] to " + _selfSelector);
            if (text.Length > 1)
            {
                Interact(elmnt =>
                {
                    elmnt.Clear();
                    elmnt.SendKeys(text);
                });

            }
            else
            {
                Interact(elmnt => elmnt.SendKeys(text));
            }
        }

        public void SendKeys(string[] keyCollextion)
        {
            string textToSend = "";
            foreach (var key in keyCollextion)
            {
                textToSend = textToSend + key;
                Interact(elmnt => elmnt.SendKeys(key));
            }
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Sending keys: [{textToSend}] to " + _selfSelector);
        }

        public void SetText(string text)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Setting text: [{text}] for " + _selfSelector);
            Interact(elmnt => elmnt.SendKeys(text));
        }

        public void Submit()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Submitting for for " + _selfSelector);
            Interact(elmnt => elmnt.Submit());
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
                return Interact(elmnt =>
                {
                    var value = elmnt.GetAttribute(attributeName) ?? string.Empty;
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
                });
        }

        public void Focus()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Focusing on element: [{_selfSelector}]");
            Interact(elmnt =>
            {
                switch (elmnt.TagName)
                {
                    case "input":
                    case "select":
                    case "textarea":
                    case "a":
                    case "iframe":
                    case "button":
                        JavaScriptExecutor.ExecuteScript("arguments[0].focus();", elmnt);
                        break;
                }
            });
            
        }

        public void Blur()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Bluring element: [{_selfSelector}]");
            Interact(elmnt => JavaScriptExecutor.ExecuteAsyncScript("arguments[0].blur();", elmnt));
        }

        public void Hover()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Hovering on element: [{_selfSelector}]");
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.MoveToElement(elmnt).Build().Perform());
        }

        public void HoverAndClick()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Hovering and clicking on element: [{_selfSelector}]");
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.MoveToElement(elmnt).Click(elmnt).Build().Perform());
        }

        public void ScrollIntoView()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Scrolloing element into view: [{_selfSelector}]");
            Interact(elmnt => JavaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", elmnt));
        }

        public string ClassName()
        {
            try
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting classname for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("class"));
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
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting name for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("name"));
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
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting id for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("id"));
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
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting style for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("style"));
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
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting value for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("value"));
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
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting type for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("type"));
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
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting title for: [{_selfSelector}]");
                return Interact(elmnt => (elmnt).GetAttribute("title"));
            }
            catch (Exception)
            {
                return null;
            }
        }


        public IWebElement GetParent()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting parent element for: [{_selfSelector}]");
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./parent::*")));
        }

        public IWebElement GetParentOfType(string type)
        {
            IWebElement element = this;
            try
            {

                do
                {
                    element = element.FindElement(By.XPath("./parent::*")); //parent relative to current element

                } while 
                (
                    element.TagName != type
                );
            }
            catch (NoSuchElementException)
            {
            }
            return element;
        }

        public IWebElement GetChild()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting child element for: [{_selfSelector}]");
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./child::*")));
        }

        public IWebElement GetPreviousSibling()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting preceding sibling element for: [{_selfSelector}]");
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./preceding-sibling::*")));
        }

        public IWebElement GetNextSibling()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting following sibling element for: [{_selfSelector}]");
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./following-sibling::*")));
        }

        public ReadOnlyCollection<IWebElement> GetElementsOfType(ElementType elementType)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Getting elements of type: [{elementType}] for " + _selfSelector);
            switch (elementType)
            {
                case ElementType.Div:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("div"));
                    }
                case ElementType.Img:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("img"));
                    }
                case ElementType.Label:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("label"));
                    }
                case ElementType.A:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("a"));
                    }
                case ElementType.Select:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("select"));
                    }
                case ElementType.Span:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("span"));
                    }
                case ElementType.Tbody:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("tbody"));
                    }
                case ElementType.Td:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("td"));
                    }
                case ElementType.Thead:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("thead"));
                    }
                case ElementType.Tr:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("tr"));
                    }
                case ElementType.Table:
                    {
                        return Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("table"));
                    }
                case ElementType.Button:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "button"));
                    }
                case ElementType.CheckBox:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "checkbox"));
                    }
                case ElementType.Radio:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("radio").Where(d => d.GetAttribute("type") == "checkbox"));
                    }
                case ElementType.Text:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "text"));
                    }
                case ElementType.Password:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "password"));
                    }
                case ElementType.Submit:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "submit"));
                    }
                case ElementType.DateTime:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "datetime"));
                    }
                case ElementType.DateTimeLocal:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "datetime-local"));
                    }
                case ElementType.Date:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "date"));
                    }
                case ElementType.Color:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "color"));
                    }
                case ElementType.Email:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "email"));
                    }
                case ElementType.Month:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "month"));
                    }
                case ElementType.Number:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "number"));
                    }
                case ElementType.Range:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "range"));
                    }
                case ElementType.Search:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "search"));
                    }
                case ElementType.Tel:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "tel"));
                    }
                case ElementType.Time:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "time"));
                    }
                case ElementType.Url:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "url"));
                    }
                case ElementType.Week:
                    {
                        return (ReadOnlyCollection<IWebElement>)Interact(elmnt => ((IFindsByTagName)elmnt).FindElementsByTagName("input").Where(d => d.GetAttribute("type") == "week"));
                    }
            }
            return null;

        }

        public void DoubleClick()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Double clicking " + _selfSelector);
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.DoubleClick(elmnt).Build().Perform());

        }

        public void RightClick()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Right clicking " + _selfSelector);
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.ContextClick(elmnt).Build().Perform());
        }

        public void ClickAndHold()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Clicking and holding " + _selfSelector);
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.ClickAndHold(elmnt).Build().Perform());
        }

        public void DragAndDrop(IWebElement targetElement)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Dragging and dropping from " + _selfSelector);
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.DragAndDrop(elmnt, targetElement).Build().Perform());
        }


        public void Click()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Clicking " + _selfSelector);
            Interact(elmnt => elmnt.Click());
        }

        public string GetAttribute(string attributeName)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting attribute value " + attributeName  + " for " + _selfSelector);
            return Interact(elmnt => elmnt.GetAttribute(attributeName));
        }

        public void SetAttribute(string attributeName, string value)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Setting atribute " + attributeName + " to " + value + " for " + _selfSelector);
            Interact(elmnt =>
            {
                JavaScriptExecutor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", elmnt, attributeName, value);
            });

        }

        public string GetCssValue(string propertyName)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting property " + propertyName + " for " + _selfSelector);
            return Interact(elmnt => elmnt.GetCssValue(propertyName));
        }

        public void SelectFromDropDownByText(string text)
        {
            Interact(elmnt =>
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Selecting drom dropdown " + _selfSelector + " for text " + text);
                var selectElement = new SelectElement(elmnt);
                selectElement.SelectByText(text);
            });
        }

        public void SelectFromDropDownByIndex(int index)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Selecting drom dropdown " + _selfSelector + " by index " + index);
            Interact(elmnt =>
            {
                if (elmnt.GetAttribute("multiple") != null)
                {
                    var optionElements = elmnt.FindElements(By.TagName("option"));
                    if (optionElements.Count > 0)
                    {
                        optionElements[index].Click();
                    }
                    else
                    {
                        var selectElement = new SelectElement(elmnt);
                        selectElement.SelectByIndex(index);
                    }
                }
                else
                {
                    var selectElement = new SelectElement(elmnt);
                    selectElement.SelectByIndex(index);
                }
            });
        }

        public void SelectFromDropDownByValue(string value)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Selecting drom dropdown " + _selfSelector + " by value " + value);
            Interact(elmnt =>
            {
                var selectElement = new SelectElement(elmnt);
                selectElement.SelectByValue(value);
            });
        }

        public void MoveToCaretPosition(int position)
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Moving to caret position " + position + " for " + _selfSelector);
            //http://stackoverflow.com/questions/499126/jquery-set-cursor-position-in-text-area
            Interact(elmnt => JavaScriptExecutor.ExecuteScript("(function(element, position) { if (element.setSelectionRange) { element.focus(); element.setSelectionRange(position, position);} else if (element.createTextRange) { var range = element.createTextRange(); range.collapse(true); range.moveEnd('character', position); range.moveStart('character', position); range.select(); } })(arguments[0],arguments[1]);", elmnt, position));
        }

        public void PerformMediaAction(MediaAction mediaAction, float arg = 0)
        {
            if (mediaAction == MediaAction.Play)
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Starting to play " + _selfSelector);
                Interact(elmnt => JavaScriptExecutor.ExecuteScript("arguments[0].play()", elmnt));
            }
            else if (mediaAction == MediaAction.Pause)
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Pausing " + _selfSelector);
                Interact(elmnt => JavaScriptExecutor.ExecuteScript("arguments[0].pause()", elmnt));
            }
            else if (mediaAction == MediaAction.VolumeChange)
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Setting volume to " + arg + " for "+  _selfSelector);
                SetAttribute("volume", "(float)" + arg);
            }
            else if (mediaAction == MediaAction.RateChange)
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Setting playback rate to " + arg + " for " + _selfSelector);
                SetAttribute("playbackRate", "(float)" + arg);
            }
            else if (mediaAction == MediaAction.RateChange)
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Changing the current time to " + arg + " for " + _selfSelector);
                SetAttribute("currentTime", "(float)" + arg);
            }
        }


        public void MouseOut()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Performing MouseOut on element: [{_selfSelector}]");
            if (WebDriverManager.TestWebDriver.UsingjQuery())
            {
                Interact(elmnt =>
                {
                    JavaScriptExecutor.ExecuteScript("$(arguments[0]).mouseout();", elmnt);
                });
            }
            else
            {
                var driver = GetDriver();
                var action = new Actions(driver);
                Interact(elmnt => action.MoveByOffset(0, 0).Build().Perform());
            }
        }

        public void MouseOver()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Performing MouseOver on element: [{_selfSelector}]");
            Interact(elmnt =>
            {
                JavaScriptExecutor.ExecuteScript("var evObj = document.createEvent('MouseEvents'); evObj.initMouseEvent(\"mouseover\", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);", elmnt);
            });

        }

        public void MouseUp()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Performing MouseOver on element: [{_selfSelector}]");
            Interact(elmnt =>
            {
                JavaScriptExecutor.ExecuteScript("var evObj = document.createEvent('MouseEvents'); evObj.initMouseEvent(\"mouseup\", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);", elmnt);
            });

        }

        public void MouseDown()
        {
            WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, $"Performing MouseOver on element: [{_selfSelector}]");
            Interact(elmnt =>
            {
                JavaScriptExecutor.ExecuteScript("var evObj = document.createEvent('MouseEvents'); evObj.initMouseEvent(\"mousedown\", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);", elmnt);
            });

        }

        public string TagName
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting tag name for " + _selfSelector);
                return Interact(elmnt => elmnt.TagName);
            }
        }

        public string Text
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting text name for " + _selfSelector);
                return Interact(elmnt => elmnt.Text);
            }
        }

        public bool Enabled
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting enabled property for " + _selfSelector);
                return Interact(elmnt => elmnt.Enabled);
            }
        }

        public bool Selected
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting selected property for " + _selfSelector);
                return Interact(elmnt => elmnt.Selected);
            }
        }

        public Point Location
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting location for " + _selfSelector);
                return Interact(elmnt => elmnt.Location);
            }
        }

        public Size Size
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting size for " + _selfSelector);
                return Interact(elmnt => elmnt.Size);
            }
        }

        public bool Displayed
        {
            get
            {
                WebDriverManager.TestWebDriver.LogMessage(LogLevel.Verbose, "Getting displayed property for " + _selfSelector);
                return Interact(elmnt => elmnt.Displayed);
            }
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


                return Interact(elmnt => unpack(elmnt));
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

        private Uri DetermineOrigin(IWebElement elmnt)
        {
            var driver = elmnt as IWrapsDriver;

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