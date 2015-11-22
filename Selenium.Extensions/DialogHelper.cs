using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

namespace Selenium.Extensions
{
    /// <summary>
    ///     Helps with file dialog interactions.
    /// </summary>
    internal static class DialogHelper
    {
        /// <summary>
        ///     Interacts with the dialog to select the specified files and return.
        /// </summary>
        /// <param name="dialogHandle">
        ///     The dialog handle.
        /// </param>
        /// <param name="comparison">Indicates how to match the <c>dialogTitle</c>.</param>
        /// <param name="dialogTitle">The dialog window title.</param>
        /// <param name="directory">
        ///     The directory.
        /// </param>
        /// <param name="files">
        ///     The files.
        /// </param>
        public static void SelectFiles(IntPtr dialogHandle, string dialogTitle, string directory, params string[] files)
        {
            // Navigate to the directory and select files - includes fallback methods if the path is longer than 259 characters
            AutomationElement appElement = AutomationElement.FromHandle(dialogHandle);
            const int keyReturn = 0x0D;
            directory = directory.AbsolutePath();

            if (files.Any())
            {
                AutomationElement fileTextBox;

                // Locate textbox
                int count = 0;
                do
                {
                    count++;
                    if (count == 4) throw new ElementNotAvailableException();
                    fileTextBox = appElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "Edit"));
                    if (fileTextBox == null) Thread.Sleep(100);
                } while (fileTextBox == null);

                var fileName = fileTextBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

                if (fileName != null)
                {
                    // Textbox accepts a maximum of 259 characters
                    if (directory.Length > 259)
                    {
                        string[] split = directory.Split('\\');
                        foreach (string value in split)
                        {
                            fileName.SetValue(value);
                            WindowHelper.SendKey(dialogHandle, keyReturn);

                            do
                            {
                                Thread.Sleep(100);
                            } while (!string.IsNullOrWhiteSpace(fileName.Current.Value));
                        }
                    }
                    else
                    {
                        fileName.SetValue(directory);
                        WindowHelper.SendKey(dialogHandle, keyReturn);
                        var fileSelected = fileTextBox.GetCurrentPattern(TextPattern.Pattern) as TextPattern;

                        while (true)
                        {
                            Thread.Sleep(100);
                            int selectedLength = 0;

                            try
                            {
                                if (fileSelected != null)
                                {
                                    selectedLength = fileSelected.GetSelection()[0].GetText(-1).Length;
                                }
                            }
// ReSharper disable EmptyGeneralCatchClause
                            catch
// ReSharper restore EmptyGeneralCatchClause
                            {
                            }

                            if (string.IsNullOrWhiteSpace(fileName.Current.Value) || selectedLength > 0)
                            {
                                break;
                            }
                        }
                    }
                }

                // Select items
                if (files.Count() == 1)
                {
                    if (fileName != null) fileName.SetValue(files.First());
                }
                else
                {
                    int tries = 0;
                    int counter = 1;
                    do
                    {
                        AutomationElementCollection children = appElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true));

                        foreach (AutomationElement child in children)
                        {
                            var itemText = (string) child.GetCurrentPropertyValue(AutomationElement.NameProperty);
                            if (files.Any(file => itemText == file))
                            {
                                ListItemSelect(child, counter != 1);
                                counter++;
                            }
                        }

                        tries++;
                        if (tries > 3) throw new ElementNotAvailableException("Could not find list items.");
                        if (counter < 2) Thread.Sleep(100);
                    } while (counter < 2);
                }
            }

            AutomationElementCollection buttons = appElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "Button"));

            InvokePattern submitButton = null;
            foreach (AutomationElement button in buttons)
            {
                string itemText = ((string) button.GetCurrentPropertyValue(AutomationElement.NameProperty)).ToLower();
                if (itemText.Contains("open") || itemText.Contains("save"))
                {
                    try
                    {
                        submitButton = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    }
                    catch
                    {
                        Point pt = button.GetClickablePoint();
                        SendInputClass.ClickLeftMouseButton((int) pt.X, (int) pt.Y);
                    }

                    break;
                }
            }

            // Do until dialog disappears
            while (WindowHelper.GetDialogHandles(dialogTitle).Any())
            {
                Thread.Sleep(100);
                if (submitButton != null)
                {
                    try
                    {
                        submitButton.Invoke();
                    }
// ReSharper disable EmptyGeneralCatchClause
                    catch
// ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }
                else
                {
                    WindowHelper.SetActiveWindow(WindowHelper.GetDesktop());
                    WindowHelper.SendKey(dialogHandle, keyReturn);
                }
            }
        }

        /// <summary>
        ///     Sets the focus to a list and selects a string item in that list.
        /// </summary>
        /// <param name="listItemElement">The list item element.</param>
        /// <param name="addToSelection">Whether to add the list item to the current selection.</param>
        /// <remarks>
        ///     This deselects any currently selected items. To add the item to the current selection
        ///     in a multiselect list, use AddToSelection instead of Select.
        /// </remarks>
        private static void ListItemSelect(AutomationElement listItemElement, bool addToSelection)
        {
            if (listItemElement != null)
            {
                SelectionItemPattern pattern;
                try
                {
                    pattern = listItemElement.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message); // Most likely "Pattern not supported."
                    return;
                }

                if (pattern != null)
                {
                    if (addToSelection)
                    {
                        pattern.AddToSelection();
                    }
                    else
                    {
                        pattern.Select();
                    }
                }
            }
        }
    }
}