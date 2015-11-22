using OpenQA.Selenium;

namespace Selenium.Extensions.Interfaces
{
    /// <summary>
    ///     Element highlighter interface.
    /// </summary>
    public interface IHighlighter
    {
        /// <summary>
        ///     Highlights or removes a highlight on an element.
        /// </summary>
        /// <param name="locator">The element locator.</param>
        /// <param name="reset">Whether to reset the element to its original state.</param>
        void Highlight(By locator, bool reset);

        /// <summary>
        ///     Highlights or removes a highlight on an element.
        /// </summary>
        /// <param name="element">The element to highlight.</param>
        /// <param name="reset">Whether to reset the element to its original state.</param>
        void HighlightElement(object element, bool reset = false);
    }
}