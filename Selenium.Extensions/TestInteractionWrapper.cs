using System;
using OpenQA.Selenium;

namespace Selenium.Extensions
{
    public static class TestInteractionWrapper
    {
        public static void Interact(ref IWebElement element, By selfSelector, Action elementLookup, Action<IWebElement> query)
        {
            Func<IWebElement, object> queryWrapper = e =>
            {
                query(e);
                return null;
            };
            Interact(ref element, selfSelector, elementLookup, queryWrapper);
        }

        public static T Interact<T>(ref IWebElement element, By selfSelector, Action elementLookup, Func<IWebElement, T> query)
        {
            try
            {
                elementLookup();
                return query(element);
            }
            catch (StaleElementReferenceException)
            {
                //Console.WriteLine("Element '{0}' is stale.", selfSelector);

                //note that this clears the element that is now stale but keeps the internal reference so we can re-resolve it to the same variable higher up
                element = null;
                return Interact(ref element, selfSelector, elementLookup, query);
            }
        }
    }
}