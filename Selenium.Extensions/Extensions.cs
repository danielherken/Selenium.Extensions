using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Extensions
{
    public static class Extensions
    {
        public static void SetTextForControl(this IWebDriver browser, By locator, string text)
        {
            WaitFor.ElementPresent(browser, locator);
            var element = browser.FindElement(locator);
            element.Clear();
            element.SendKeys(text);
        }

        public static void SelectYesNoRadioButton(this IWebDriver browser, By locator, bool isSelected)
        {
            WaitFor.ElementPresent(browser, locator);
            var elements = browser.FindElements(locator);

            if (isSelected)
            {
                elements[0].Click();
            }
            else
            {
                elements[1].Click();
            }
        }

        public static void SelectRadioButton(this IWebDriver browser, By locator, int index)
        {
            WaitFor.ElementPresent(browser, locator);
            var radioButtons = browser.FindElements(locator);
            radioButtons[index].Click();
        }

        public static void SelectDropDownByText(this IWebDriver browser, By locator, string text)
        {
            WaitFor.ElementPresent(browser, locator);
            var selectElement = new SelectElement(browser.FindElement(locator));
            selectElement.SelectByText(text);
        }

        public static void SelectCheckBox(this IWebDriver browser, By locator, string label)
        {
            WaitFor.ElementPresent(browser, locator);
            var checkBox = browser.FindElement(locator);
            checkBox.Click();
        }

        public static void ClickLink(this IWebDriver browser, By locator)
        {
            WaitFor.ElementPresent(browser, locator);
            var element = browser.FindElement(locator);
            element.Click();
        }

        public static void ClickNextButton(this ISearchContext browser)
        {
            var element = browser.FindElement(By.ClassName("next"));
            element.Click();
        }

        public static void ClickSubmitButton(this ISearchContext browser)
        {
            var element = browser.FindElement(By.XPath("//input[@type='submit']"));
            element.Click();
        }

        public static void FindTextOnPage(this ISearchContext browser, string textToFind)
        {
            var validationMessageLocator = By.XPath(string.Format("//*[contains(.,'{0}')]", textToFind));
            WaitFor.ElementPresent((IWebDriver)browser, validationMessageLocator);
        }
    }
}
