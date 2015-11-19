using System;
using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;

namespace Selenium.Extensions
{
    public interface ITestWebElement
    {
        ICoordinates Coordinates { get; }
        bool Displayed { get; }
        bool Enabled { get; }
        Point Location { get; }
        Point LocationOnScreenOnceScrolledIntoView { get; }
        bool Selected { get; }
        Size Size { get; }
        string TagName { get; }
        string Text { get; }
        IWebElement WrappedElement { get; }

        void Clear();
        void Click();
        IWebElement FindElement(By by);
        IWebElement FindElementByClassName(string className);
        IWebElement FindElementByCssSelector(string cssSelector);
        IWebElement FindElementById(string id);
        IWebElement FindElementByName(string name);
        IWebElement FindElementByPartialLinkText(string partialLinkText);
        IWebElement FindElementByTagName(string tagName);
        IWebElement FindElementByXPath(string xpath);
        ReadOnlyCollection<IWebElement> FindElements(By by);
        IWebElement FindElementOfType(ElementType elementType);
        ReadOnlyCollection<IWebElement> FindElementsByClassName(string className);
        ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector);
        ReadOnlyCollection<IWebElement> FindElementsById(string id);
        ReadOnlyCollection<IWebElement> FindElementsByName(string name);
        ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText);
        ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName);
        ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath);
        string GetAttribute(string attributeName);
        string GetCssValue(string propertyName);
        Screenshot GetScreenshot();
        void SendKeys(string text);
        void SetText(string text);
        void Submit();
        IWebDriver GetDriver();
        IJavaScriptExecutor GetJavascriptExecutor();
        T GetAttributeAsType<T>(string attributeName);
        void Focus();
        void Blur();
        void Hover();
        void HoverAndClick();
        void ScrollIntoView();
        string ClassName();
        string Name();
        string Id();
        string Style();
        string Value();
        string TypeValue();
        string Title();
        IWebElement GetParent();
        IWebElement GetChild();
        IWebElement GetPreviousSibling();
        IWebElement GetNextSibling();
        ReadOnlyCollection<IWebElement> GetElementsOfType(ElementType elementType);
        void DoubleClick();
        void RightClick();
        void ClickAndHold();
        void DragAndDrop(IWebElement targetElement);
        void SelectFromDropDownByText(string text);
        void SelectFromDropDownByIndex(int index);
        void SelectFromDropDownByValue(string value);


    }
}