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

        /// <summary>
        /// Finds the element by class name.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns></returns>
        public IWebElement FindElementByClassName(string className)
        {
            return Interact(elmnt => ((IFindsByClassName) elmnt).FindElementByClassName(className));
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
            return Interact(elmnt => ((IFindsByClassName) elmnt).FindElementsByClassName(className));
        }

        public IWebElement FindElementByCssSelector(string cssSelector)
        {
            return Interact(elmnt => ((IFindsByCssSelector) elmnt).FindElementByCssSelector(cssSelector));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            return Interact(elmnt => ((IFindsByCssSelector) elmnt).FindElementsByCssSelector(cssSelector));
        }

        public IWebElement FindElementById(string id)
        {
            return Interact(elmnt => ((IFindsById) elmnt).FindElementById(id));
        }

        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            return Interact(elmnt => ((IFindsById) elmnt).FindElementsById(id));
        }


        IWebElement IFindsByLinkText.FindElementByLinkText(string linkText)
        {
            return Interact(elmnt =>
            {
                var e = ((IFindsByLinkText) elmnt);
                return e.FindElementByLinkText(linkText);
            });
        }

        ReadOnlyCollection<IWebElement> IFindsByLinkText.FindElementsByLinkText(string linkText)
        {
            return Interact(elmnt =>
            {
                var e = ((IFindsByLinkText) elmnt);
                return e.FindElementsByLinkText(linkText);
            });
        }

        public IWebElement FindElementByName(string name)
        {
            return Interact(elmnt => ((IFindsByName) elmnt).FindElementByName(name));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            return Interact(elmnt => ((IFindsByName) elmnt).FindElementsByName(name));
        }

        public IWebElement FindElementByPartialLinkText(string partialLinkText)
        {
            return Interact(elmnt => ((IFindsByPartialLinkText) elmnt).FindElementByPartialLinkText(partialLinkText));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
        {
            return Interact(elmnt => ((IFindsByPartialLinkText) elmnt).FindElementsByPartialLinkText(partialLinkText));
        }

        public IWebElement FindElementByTagName(string tagName)
        {
            return Interact(elmnt => ((IFindsByTagName) elmnt).FindElementByTagName(tagName));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            return Interact(elmnt => ((IFindsByTagName) elmnt).FindElementsByTagName(tagName));
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            return Interact(elmnt => ((IFindsByXPath) elmnt).FindElementByXPath(xpath));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            return Interact(elmnt => ((IFindsByXPath) elmnt).FindElementsByXPath(xpath));
        }

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
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
                return Interact(elmnt =>
                {
                    var locatable = (ILocatable) elmnt;
                    return locatable.Coordinates;
                });
            }
        }

        public Screenshot GetScreenshot()
        {
            return Interact(elmnt => ((ITakesScreenshot) elmnt).GetScreenshot());
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
            Interact(elmnt => elmnt.Clear());
        }

        public void SendKeys(string text)
        {
            Interact(elmnt => elmnt.SendKeys(text));
        }

        public void SetText(string text)
        {
            Interact(elmnt => elmnt.SendKeys(text));
        }

        public void Submit()
        {
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
            Interact(elmnt => action.MoveToElement(elmnt).Build().Perform());
        }

        public void HoverAndClick()
        {
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.MoveToElement(elmnt).Click(elmnt).Build().Perform());
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
                return Interact(elmnt => (elmnt).GetAttribute("title"));
            }
            catch (Exception)
            {
                return null;
            }
        }


        public IWebElement GetParent()
        {
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./parent::*")));
        }

        public IWebElement GetChild()
        {
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./child::*")));
        }

        public IWebElement GetPreviousSibling()
        {
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./preceding-sibling::*")));
        }

        public IWebElement GetNextSibling()
        {
            return Interact(elmnt => (elmnt).FindElement(By.XPath("./following-sibling::*")));
        }

        public ReadOnlyCollection<IWebElement> GetElementsOfType(ElementType elementType)
        {
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
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.DoubleClick(elmnt).Build().Perform());

        }

        public void RightClick()
        {
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.ContextClick(elmnt).Build().Perform());
        }

        public void ClickAndHold()
        {
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.ClickAndHold(elmnt).Build().Perform());
        }

        public void DragAndDrop(IWebElement targetElement)
        {
            var driver = GetDriver();
            var action = new Actions(driver);
            Interact(elmnt => action.DragAndDrop(elmnt, targetElement).Build().Perform());
        }


        public void Click()
        {
            Interact(elmnt => elmnt.Click());
        }

        public string GetAttribute(string attributeName)
        {
            return Interact(elmnt => elmnt.GetAttribute(attributeName));
        }

        public string GetCssValue(string propertyName)
        {
            return Interact(elmnt => elmnt.GetCssValue(propertyName));
        }

        public void SelectFromDropDownByText(string text)
        {
            Interact(elmnt =>
            {
                var selectElement = new SelectElement(elmnt);
                selectElement.SelectByText(text);
            });
        }

        public void SelectFromDropDownByIndex(int index)
        {
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
            Interact(elmnt =>
            {
                var selectElement = new SelectElement(elmnt);
                selectElement.SelectByValue(value);
            });
        }

        public string TagName
        {
            get { return Interact(elmnt => elmnt.TagName); }
        }

        public string Text
        {
            get { return Interact(elmnt => elmnt.Text); }
        }

        public bool Enabled
        {
            get { return Interact(elmnt => elmnt.Enabled); }
        }

        public bool Selected
        {
            get { return Interact(elmnt => elmnt.Selected); }
        }

        public Point Location
        {
            get { return Interact(elmnt => elmnt.Location); }
        }

        public Size Size
        {
            get { return Interact(elmnt => elmnt.Size); }
        }

        public bool Displayed
        {
            get { return Interact(elmnt => elmnt.Displayed); }
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