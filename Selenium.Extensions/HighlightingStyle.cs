using System;
using System.Drawing;

namespace Selenium.Extensions
{
    public class HighlightingStyle
    {
        private string _backgroundColor;
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                try
                {
                    ColorTranslator.FromHtml(value);
                }
                catch (Exception)
                {
                    throw new ArgumentException("The color " + value + " is not a valid background html color");
                }
                _backgroundColor = value;
            }
        }


        public int BorderSizeInPixels { get; set; }

        public BorderStyle BorderStyle { get; set; }

        private string _borderColor;
        public string BorderColor
        {
            get { return _borderColor; }
            set
            {
                try
                {
                    ColorTranslator.FromHtml(value);
                }
                catch (Exception)
                {
                    throw new ArgumentException("The color " + value + " is not a valid border html color");
                }
                _borderColor = value;
            }
        }
    }

    public enum BorderStyle
    {
        None,
        Hidden,
        Dotted,
        Dashed,
        Solid,
        Double,
        Groove,
        Ridge,
        Inset,
        Outset,
        Initial,
        Inherit
    }

}
