using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using Selenium.Extensions.Interfaces;

namespace Selenium.Extensions
{
    public class ScreenShotExtensions
    {

        /// <summary>
        /// Gets the full screen shot.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        public static Bitmap GetFullScreenShot(ITestWebDriver driver, TestSettings testSettings)
        {
            Bitmap stitchedImage = null;
            try
            {
                var totalwidth1 = (long) (driver.ExecuteScript("return document.body.offsetWidth"));
                var totalHeight1 = (long) (driver.ExecuteScript("return  document.body.parentNode.scrollHeight"));
                var totalWidth = (int) totalwidth1;
                var totalHeight = (int) totalHeight1;
                // Get the Size of the Viewport
                var viewportWidth1 = (long) (driver.ExecuteScript("return document.body.clientWidth"));
                var viewportHeight1 = (long) (driver.ExecuteScript("return window.innerHeight"));
                var viewportWidth = (int) viewportWidth1;
                var viewportHeight = (int) viewportHeight1;
                // Split the Screen in multiple Rectangles
                var rectangles = new List<Rectangle>();
                // Loop until the Total Height is reached
                for (int i = 0; i < totalHeight; i += viewportHeight)
                {
                    int newHeight = viewportHeight;
                    // Fix if the Height of the Element is too big
                    if (i + viewportHeight > totalHeight)
                    {
                        newHeight = totalHeight - i;
                    }
                    // Loop until the Total Width is reached
                    for (int ii = 0; ii < totalWidth; ii += viewportWidth)
                    {
                        int newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (ii + viewportWidth > totalWidth)
                        {
                            newWidth = totalWidth - ii;
                        }

                        // Create and add the Rectangle
                        var currRect = new Rectangle(ii, i, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }
                // Build the Image
                stitchedImage = new Bitmap(totalWidth, totalHeight);
                // Get all Screenshots and stitch them together
                Rectangle previous = Rectangle.Empty;
                //int numb = 0;
                
                foreach (Rectangle rectangle in rectangles)
                {
                    if (testSettings.DriverType == WebDriverType.ChromeDriver)
                    {
                        // Calculate the Scrolling (if needed)
                        if (previous != Rectangle.Empty)
                        {
                            int xDiff = rectangle.Right - previous.Right;
                            int yDiff = rectangle.Bottom - previous.Bottom;
                            // Scroll
                            driver.ExecuteScript($"window.scrollBy({xDiff}, {yDiff})");

                            Thread.Sleep(1000);
                        }
                    }
                    // Take Screenshot
                    Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    //numb++;
                    //screenshot.SaveAsFile(@"\\psf\Home\Documents\Hack\" + numb + ".png",ImageFormat.Png);
                    //
                    //screenshot.SaveAsFile("C:\\Backup\\" + numb + ".png",ImageFormat.Png);
                    //numb ++;
                    // Build an Image out of the Screenshot
                    Image screenshotImage;
                    using (var memStream = new MemoryStream(screenshot.AsByteArray))
                    {
                        screenshotImage = Image.FromStream(memStream);
                    }
                    // Calculate the Source Rectangle
                    var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
                    // Copy the Image
                    using (Graphics g = Graphics.FromImage(stitchedImage))
                    {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }
                    // Set the Previous Rectangle
                    previous = rectangle;
                }
            }
            catch
            {
                // handle
            }
            if (testSettings.DriverType == WebDriverType.ChromeDriver)
            {
                driver.ExecuteScript("window.scrollBy(0, 0)");
            }
            return stitchedImage;
        }

    }
}