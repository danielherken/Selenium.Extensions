using System;
using OpenQA.Selenium;
using Selenium.Extensions;
using Selenium.Extensions.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace MultiBrowser.Test.SiteTest
{
    /// <summary>
    ///  SiteTest for "http://test.multibrowser.com/mb_aft_rec_web/"
    /// </summary>
    public class Example : IDisposable
    {
        private readonly TestSettings _testSettings;
        /// <summary>
        /// The web _driver
        /// </summary>
        private ITestWebDriver _driver;
        /// <summary>
        /// The _test output helper
        /// </summary>
        private readonly ITestOutputHelper _testOutputHelper;

        public Example(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _testSettings = TestSettings.Default;
            _testSettings.TestUri = new Uri("http://test.multibrowser.com/mb_aft_rec_web/");
            _testSettings.LogScreenShots = true;
            _testSettings.LogLevel = LogLevel.Verbose;
            _testSettings.HighlightElements = true;
            _testSettings.TimeoutTimeSpan = new TimeSpan(0, 0, 30);
            _testSettings.DeleteAllCookies = false;
            _testSettings.MaximiseBrowser = true;
            _testSettings.TestDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Tests\\";
        }

        /// <summary>
        ///  Runs the predefined installed browser tests for http://test.multibrowser.com/mb_aft_rec_web/.
        /// </summary>
        /// <param name="driverType">The selenium web driver type to use.</param>
        /// <exception cref="NullReferenceException">_driver cannot be null</exception>
        [Theory(DisplayName = "Installed Browser")]
        [InlineData(WebDriverType.ChromeDriver)]
        [InlineData(WebDriverType.InternetExplorerDriver)]
        public void RunInstalledBrowserTests(WebDriverType driverType)
        {
            _testSettings.TestType = TestType.InstalledBrowser;
            _testSettings.DriverType = driverType;
            _testSettings.TestDirectory = _testSettings.TestDirectory + "\\Installed Browsers\\";
            _driver = WebDriverManager.InitializeInstalledBrowserDriver(_testSettings, _testOutputHelper);
            if (_driver == null)
            {
                throw new NullReferenceException("_driver cannot be null");
            }

            PerformTest();

            _driver.Close();
        }

        /// <summary>
        /// Runs the predefined standalone browser tests for http://test.multibrowser.com/mb_aft_rec_web/.
        /// </summary>
        /// <param name="driverType">The selenium web driver to use.</param>
        /// <param name="version">The browser version to use. .</param>
        /// <exception cref="NullReferenceException">_driver cannot be null</exception>
        [Theory(DisplayName = "Standalone Browser")]
        [InlineData(WebDriverType.ChromeDriver, 44)]
        [InlineData(WebDriverType.FirefoxDriver, 40)]
        public void RunStandaloneBrowserTests(WebDriverType driverType, decimal version)
        {
            _testSettings.TestType = TestType.StandaloneBrowser;
            _testSettings.DriverType = driverType;
            _testSettings.TestDirectory = _testSettings.TestDirectory + "\\Standalone Browsers\\";
            _driver = WebDriverManager.InitializeStandaloneBrowserDriver(_testSettings, version, _testOutputHelper);
            if (_driver == null)
            {
                throw new NullReferenceException("_driver cannot be null");
            }

            PerformTest();

            _driver.Close();
        }

        /// <summary>
        /// Runs the predefined MultiBrowser emulator tests for http://test.multibrowser.com/mb_aft_rec_web/.
        /// </summary>
        /// <param name="emulator">The emulator.</param>
        /// <param name="orientation">The orientation.</param>
        /// <exception cref="NullReferenceException">_driver cannot be null</exception>
        [Theory(DisplayName = "MultiBrowser Emulator")]
        [InlineData(Emulator.iPad, DeviceOrientation.Portrait)]
        [InlineData(Emulator.iPhone6, DeviceOrientation.Portrait)]
        [InlineData(Emulator.SonyXperiaZ3, DeviceOrientation.Portrait)]
        [InlineData(Emulator.SamsungGalaxyS6, DeviceOrientation.Portrait)]
        public void RunMultiBrowserEmulatorTests(Emulator emulator, DeviceOrientation orientation)
        {
            _testSettings.TestType = TestType.EmulatorBrowser;
            _testSettings.DriverType = WebDriverType.ChromeDriver;
            _testSettings.TestDirectory = _testSettings.TestDirectory + "\\Emulator Browsers\\";
            _driver = WebDriverManager.InitializeMultiBrowserEmulatorDriver(_testSettings, emulator, orientation, _testOutputHelper);
            if (_driver == null)
            {
                throw new NullReferenceException("_driver cannot be null");
            }

            PerformTest();

            _driver.Close();
        }

        /// <summary>
        /// Performs the test.
        /// </summary>
        public void PerformTest()
        {
            _driver.Navigate().GoToUrl(new Uri("http://test.multibrowser.com/mb_aft_rec_web/"));
            _driver.WaitForPageToLoad();
            var inputtext1 = _driver.FindElement(By.Id("input-text")) as ITestWebElement;
            inputtext1.Focus();
            inputtext1.SendKeys(new[] { "T", "h", "i", "s", " ", "a", " ", "t", "e", "s", "t" });
            inputtext1.SendKeys("This w a test");
            inputtext1.MoveToCaretPosition(6); inputtext1.SendKeys("a");
            inputtext1.MoveToCaretPosition(7); inputtext1.SendKeys("s");
            inputtext1.SendKeys(new[] { " ", "b", "u", "t", " ", "i", "s", "n", "t" });
            var inputpassword1 = _driver.FindElement(By.Id("input-password")) as ITestWebElement;
            inputpassword1.Focus();
            inputpassword1.SendKeys(new[] { "p", "a", "s", "s" });
            var inputemail1 = _driver.FindElement(By.Id("input-email")) as ITestWebElement;
            inputemail1.Focus();
            inputemail1.SendKeys("test@email.com");
            var inputsearch1 = _driver.FindElement(By.Id("input-search")) as ITestWebElement;
            inputsearch1.Focus();
            inputsearch1.SendKeys(new[] { "S", "e", "a", "r", "c", "h" });
            var inputtel1 = _driver.FindElement(By.Id("input-tel")) as ITestWebElement;
            inputtel1.Focus();
            inputtel1.SendKeys("1005000");
            var inputurl1 = _driver.FindElement(By.Id("input-url")) as ITestWebElement;
            inputurl1.Focus();
            inputurl1.SendKeys(new[] { "h", "t", "t", "p", ":", "/", "/", "w", "w", "w", ".", "s", "i", "t", "e", ".", "c", "o", "m" });
            var inputcolor1 = _driver.FindElement(By.Id("input-color")) as ITestWebElement;
            inputcolor1.SetText("#008080");
            var inputdate1 = _driver.FindElement(By.Id("input-date")) as ITestWebElement;
            inputdate1.Focus();
            inputdate1.SetText("2015-11-03");
            var inputdatetimelocal1 = _driver.FindElement(By.Id("input-datetime-local")) as ITestWebElement;
            inputdatetimelocal1.Focus();
            var inputmonth1 = _driver.FindElement(By.Id("input-month")) as ITestWebElement;
            inputmonth1.Focus();
            inputmonth1.SetText("2015-11");
            var inputnumber1 = _driver.FindElement(By.Id("input-number")) as ITestWebElement;
            inputnumber1.Focus();
            inputnumber1.SendKeys("2");
            var inputrange1 = _driver.FindElement(By.Id("input-range")) as ITestWebElement;
            inputrange1.Focus();
            inputrange1.SetText("9");
            var inputtime1 = _driver.FindElement(By.Id("input-time")) as ITestWebElement;
            inputtime1.Focus();
            inputtime1.SetText("01:04");
            var inputweek1 = _driver.FindElement(By.Id("input-week")) as ITestWebElement;
            inputweek1.Focus();
            inputweek1.SetText("2015-W46");
            var select2 = _driver.FindElement(By.Id("input-select")) as ITestWebElement;
            select2.SelectFromDropDownByIndex(2);
            var select1 = _driver.FindElement(By.Id("input-select-multipe")) as ITestWebElement;
            select1.SelectFromDropDownByIndex(1);
            var inputradio1 = _driver.FindElement(By.Id("input-radio-2")) as ITestWebElement;
            inputradio1.Focus();
            inputradio1.Click();
            var inputcheckbox1 = _driver.FindElement(By.Id("input-checkbox-2")) as ITestWebElement;
            inputcheckbox1.Click();
            var inputfile1 = _driver.FindElement(By.Id("input-file")) as ITestWebElement;
            inputfile1.Click();
            _driver.SelectFile(@"C:\Windows", new[] { "win.ini" });
            var textarea1 = _driver.FindElement(By.Id("input-textarea")) as ITestWebElement;
            textarea1.Focus();
            textarea1.SendKeys("This is a line" + Environment.NewLine + "And another");
            inputdatetimelocal1.Focus();
            inputdatetimelocal1.SetText("2015-11-13T01:04");
            var inputsubmit1 = _driver.FindElement(By.Id("input-submit")) as ITestWebElement;
            inputsubmit1.Click();
            _driver.WaitForPageToLoad();
            _driver.Wait(TimeSpan.FromSeconds(2));
            var video1 = _driver.FindElement(By.CssSelector("#content > FORM:nth-child(1) > SECTION:nth-child(3) > VIDEO:nth-child(2)")) as ITestWebElement;
            video1.PerformMediaAction(MediaAction.Play);
            _driver.Wait(TimeSpan.FromSeconds(2));
            video1.PerformMediaAction(MediaAction.Seeked, (float)2.287324);
            _driver.Wait(TimeSpan.FromSeconds(2));
            video1.PerformMediaAction(MediaAction.Seeked, (float)8.844319);
            _driver.Wait(TimeSpan.FromSeconds(2));
            video1.PerformMediaAction(MediaAction.Play);
            video1.PerformMediaAction(MediaAction.VolumeChange, (float)1);
            video1.PerformMediaAction(MediaAction.Pause);
            var audio1 = _driver.FindElement(By.CssSelector("#content > FORM:nth-child(1) > SECTION:nth-child(3) > AUDIO:nth-child(3)")) as ITestWebElement;
            audio1.PerformMediaAction(MediaAction.Play);
            _driver.Wait(TimeSpan.FromSeconds(2));
            audio1.PerformMediaAction(MediaAction.Pause);
            var inputtext3 = _driver.FindElement(By.CssSelector("#content > FORM:nth-child(1) > SECTION:nth-child(4) > DIV:nth-child(3) > INPUT:nth-child(2)")) as ITestWebElement;
            inputtext3.Focus();
            inputtext3.SendKeys(new[] { "c", "s", "s" });
            _driver.SwitchToIframe(0, By.CssSelector("HTML > BODY:nth-child(2) > DIV:nth-child(1) > DIV:nth-child(2) > FORM:nth-child(1) > SECTION:nth-child(1) > FIELDSET:nth-child(2) > DIV:nth-child(1) > INPUT:nth-child(2)"));
            var inputtext4 = _driver.FindElement(By.CssSelector("HTML > BODY:nth-child(2) > DIV:nth-child(1) > DIV:nth-child(2) > FORM:nth-child(1) > SECTION:nth-child(1) > FIELDSET:nth-child(2) > DIV:nth-child(1) > INPUT:nth-child(2)")) as ITestWebElement;
            inputtext4.Focus();
            inputtext4.SendKeys(new[] { "T", "y", "p", "e", " ", "i", "n", " ", "a", " ", "i", "F", "r", "a", "m", "e" });
            _driver.SwitchTo().DefaultContent();
            var inputbutton1 = _driver.FindElement(By.Id("window-alert")) as ITestWebElement;
            inputbutton1.Click();
            _driver.SwitchToAlert(AlertAction.Accept);
            var inputbutton3 = _driver.FindElement(By.Id("window-confirm")) as ITestWebElement;
            inputbutton3.Click();
            _driver.SwitchToAlert(AlertAction.Dismiss);
            var inputbutton2 = _driver.FindElement(By.Id("window-prompt")) as ITestWebElement;
            inputbutton2.Click();
            _driver.SwitchToPrompt(AlertAction.Accept, "Prompt Text");
            var inputtext5 = _driver.FindElement(By.Id("input-ajax")) as ITestWebElement;
            inputtext5.Focus();
            audio1.PerformMediaAction(MediaAction.Pause);
            inputtext5.SendKeys(new[] { "N", "e", "w", " " });
            _driver.Wait(TimeSpan.FromSeconds(2));
            var li2 = _driver.FindElement(By.CssSelector("#home-ajax-suggestions > LI:nth-child(2)")) as ITestWebElement;
            li2.Click();
            _driver.Wait(TimeSpan.FromSeconds(2));
            Assert.Equal("New York", inputtext5.Text);
            //End Of Test

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _driver?.Quit();
            }
        }
    }
}

