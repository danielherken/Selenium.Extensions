using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Selenium.Extensions
{
    public interface ITestWebDriver
    {
        ICapabilities Capabilities { get; }
        string CurrentWindowHandle { get; }
        IFileDetector FileDetector { get; set; }
        IKeyboard Keyboard { get; }
        IMouse Mouse { get; }
        string PageSource { get; }
        TestSettings Settings { get; }
        string Title { get; }
        string Url { get; set; }
        ReadOnlyCollection<string> WindowHandles { get; }

        void Close();
        void Dispose();
        object ExecuteAsyncScript(string script, params object[] args);
        object ExecuteScript(string script, params object[] args);
        IWebElement FindElement(By by);
        ReadOnlyCollection<IWebElement> FindElements(By by);
        Screenshot GetScreenshot();
        IOptions Manage();
        INavigation Navigate();
        void Quit();
        ITargetLocator SwitchTo();
        string GetText();
        bool HasElement(By locator);
        void WaitForPageToLoad(TimeSpan? timeSpan = null);
        void WaitUntiDisplayed(By locator, TimeSpan timeout);
        void WaitUntilVisible(By locator, TimeSpan timeout);
        void ScrollDownPage();
        IWebElement FindElementOfType(ElementType elementType);
        ReadOnlyCollection<IWebElement> FindElementsOfType(ElementType elementType);
        bool IsAlertPresent();
        IWebElement SelectElementByText(By locator, string text);
        ReadOnlyCollection<IWebElement> SelectElementsByText(By locator, string text);
        void WaitForAjax();
        IWebElement WaitElement(By locator);
        IWebElement WaitforElement(By locator, TimeSpan timeSpan);
        IWebElement WaitElementDisappear(By locator);
        IWebElement WaitElementDisappear(By locator, TimeSpan timeSpan);
        IWebElement WaitElementIsInvisible(By locator);
        IWebElement WaitTillElementIsInvisible(By locator, TimeSpan timeSpan);
        IWebElement WaitTillElementIsVisible(By locator);
        IWebElement WaitTillElementIsVisible(By locator, TimeSpan timeSpan);
        IWebElement SelectElementByAttribute(By locator, string attribute, string value);
        IReadOnlyCollection<IWebElement> SelectElementsByAttribute(By locator, string attribute, string value);
    }
}