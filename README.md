Selenium Web Driver Extensions
==============================
<p>Contains a set of functions built around selenium to make writing and running tests easier. There are 4 types of tests defined in the sample. You will need a evaluation version of MultiBrowser available from http://multibrowser.com to run the Standalone Browsers and Emulator browser tests.You can run the Installed and SourceLabs cloud tests with no additional software. To prepare your test enviroment you will need to run the following commands in the Nuget Package Manager Console:</p>

- PM> Install-Package xunit
- PM> Install-Package xunit.runner.visualstudio
- PM> Install-Package Selenium.WebDriver
- PM> Install-Package Selenium.Support
- PM> Install-Package Selenium.WebDriver.ChromeDriver
- PM> Install-Package Selenium.WebDriver.IEDriver
- PM> Install-Package Selenium.WebDriver.MicrosoftWebDriver

<p>A easier way to create tests if you want to test MultiBrowser and run multiple virtualised browser tests locally, is to download MultiBrowser and just select the create and record test from the Visual Studio menu. This will create your test project for you with all the dependencies installed automatically.
The edge web driver and test will require you to be running windows 10. To test the sauce labs intergration, regerster a test account at https://saucelabs.com/</p>

<b>What the test below does</b></br>
In the test below, a real world test is run. You can choose which test to run by selecting the test explorer in visual studio and running the Installed Chrome test. This test highlights the following:</br>
<ol>
<li>Easily creating the relevant selenium driver with the specified settings</li>
<li> Opening of a webpage</li>
<li>Waiting for the page to finish loading.</li>
<li>Finding elements by Id, Type and Css Selectors.</li>
<li>Typing in real worl scenarios by typing key by key. You can send the letters in 1 go if testing on the cloud which is slower.</li>
<li>Setting various Html5 input values</li>
<li>Simulationing a user correcting themselves. See the MoveToCaretPosition for a example.</li>
<li>Seamlessly switching in and out of a iframe.</li>
<li>Handling various alerts and prompts. </li>
</ol>
Ill add how to play and stop video etc this weekend :smile:
```
using System;
using OpenQA.Selenium;
using Xunit;

namespace Selenium.Extensions
{
    /// <summary>
    ///  SiteTest for "http://test.multibrowser.com/mb_aft_rec_web/"
    /// </summary>
    public class SampleTest : IDisposable
    {
        /// <summary>
        /// The web _driver
        /// </summary>
        private ITestWebDriver _driver;

        /// <summary>
        ///  Runs the predefined installed browser tests for http://test.multibrowser.com/mb_aft_rec_web/.
        /// </summary>
        /// <param name="driverType">The selenium web driver type to use.</param>
        /// <exception cref="NullReferenceException">_driver cannot be null</exception>
        [Theory(DisplayName = "Installed Browser")]
        [InlineData(WebDriverType.ChromeDriver)]
        //[InlineData(WebDriverType.EdgeDriver)]
        [InlineData(WebDriverType.InternetExplorerDriver)]
        public void RunInstalledBrowserTests(WebDriverType driverType)
        {
            var testSettings = TestSettings.Default;
            testSettings.TestType = TestType.InstalledBrowser;
            testSettings.DriverType = driverType;
            testSettings.TimeoutTimeSpan = new TimeSpan(0, 0, 30);
            testSettings.DeleteAllCookies = true;
            testSettings.MaximiseBrowser = true;
            testSettings.TestUri = new Uri("http://test.multibrowser.com/mb_aft_rec_web/");
            testSettings.LogScreenShots = true;
            testSettings.LogEvents = false;
            testSettings.LogLevel = LogLevel.Errors;
            testSettings.HighlightElements = false;
            testSettings.TestDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                         "\\Tests\\Installed Browsers\\";
            _driver = GetWebDriver.GetInstalledBrowserWebDriver(testSettings);
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
            var testSettings = TestSettings.Default;
            testSettings.TestType = TestType.StandaloneBrowser;
            testSettings.DriverType = driverType;
            testSettings.TimeoutTimeSpan = new TimeSpan(0, 0, 30);
            testSettings.DeleteAllCookies = true;
            testSettings.MaximiseBrowser = true;
            testSettings.TestUri = new Uri("http://test.multibrowser.com/mb_aft_rec_web/");
            testSettings.LogScreenShots = true;
            testSettings.LogEvents = false;
            testSettings.LogLevel = LogLevel.Errors;
            testSettings.HighlightElements = false;
            testSettings.TestDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Tests\\Standalone Browsers\\";
            _driver = GetWebDriver.GetStandaloneWebDriver(testSettings, version);
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
        [InlineData(Emulator.iPhone6Plus, DeviceOrientation.Portrait)]
        [InlineData(Emulator.SamsungGalaxyS6, DeviceOrientation.Portrait)]
        public void RunMultiBrowserEmulatorTests(Emulator emulator, DeviceOrientation orientation)
        {
            var testSettings = TestSettings.Default;
            testSettings.TestType = TestType.EmulatorBrowser;
            testSettings.DriverType = WebDriverType.ChromeDriver;
            testSettings.TimeoutTimeSpan = new TimeSpan(0, 0, 30);
            testSettings.DeleteAllCookies = true;
            testSettings.MaximiseBrowser = true;
            testSettings.TestUri = new Uri("http://test.multibrowser.com/mb_aft_rec_web/");
            testSettings.LogScreenShots = true;
            testSettings.LogEvents = false;
            testSettings.LogLevel = LogLevel.Errors;
            testSettings.HighlightElements = false;
            testSettings.TestDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Tests\\Emulator Browsers\\";
            _driver = GetWebDriver.GetMultiBrowserEmulatorWebDriver(testSettings, emulator, orientation);
            if (_driver == null)
            {
                throw new NullReferenceException("_driver cannot be null");
            }

            PerformTest();

            _driver.Close();
        }

        /// <summary>
        /// Runs the predifined Sauce Labs browser tests for http://test.multibrowser.com/mb_aft_rec_web/.
        /// </summary>
        /// <param name="browserName">The full browser name.</param>
        /// <param name="os">The operating system.</param>
        /// <param name="apiName">The api name.</param>
        /// <param name="device">The device name.</param>
        /// <param name="version">The version name.</param>
        /// <param name="deviceOrientation">The device orientation.</param>
        /// <exception cref="NullReferenceException">_driver cannot be null</exception>
        [Theory(DisplayName = "SauceLabs Cloud")]
        [InlineData("Android 5.1", "Linux", "android", "Android", "5.1", DeviceOrientation.Portrait)]
        [InlineData("iPad 9.0", "Mac 10.10", "ipad", "iPad", "9.0", DeviceOrientation.Portrait)]
        [InlineData("iPhone 9.0", "Mac 10.10", "iphone", "iPhone", "9.0", DeviceOrientation.Portrait)]
        [InlineData("Chrome 43 (Linux)", "Linux", "chrome", "Google Chrome", "43", DeviceOrientation.Portrait)]
        [InlineData("Firefox 37 (Yosemite)", "Mac 10.10", "firefox", "Firefox", "37", DeviceOrientation.Portrait)]
        [InlineData("Firefox 18 (Mavericks)", "Mac 10.9", "firefox", "Firefox", "18", DeviceOrientation.Portrait)]
        [InlineData("Firefox 42 (Windows 2003)", "Windows 2003", "firefox", "Firefox", "42", DeviceOrientation.Portrait)]
        [InlineData("Chrome 27 (Windows 10)", "Windows 10", "chrome", "Google Chrome", "27", DeviceOrientation.Portrait)]
        public void RunSauceLabsBrowserTests(string browserName, string os, string apiName, string device, string version, DeviceOrientation deviceOrientation)
        {
            var testSettings = TestSettings.Default;
            testSettings.TestType = TestType.SauceLabsBrowsers;
            testSettings.DriverType = WebDriverType.RemoteWebDriver;
            testSettings.TimeoutTimeSpan = new TimeSpan(0, 0, 60);
            testSettings.DeleteAllCookies = true;
            testSettings.MaximiseBrowser = true;
            testSettings.TestUri = new Uri("http://test.multibrowser.com/mb_aft_rec_web/");
            testSettings.LogScreenShots = true;
            testSettings.LogEvents = false;
            testSettings.LogLevel = LogLevel.Errors;
            testSettings.HighlightElements = false;
            testSettings.TestDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Tests\\SauceLabs Browsers\\";
            var hubSettings = new HubSettings
            {
                HubUsername = SauceLabsCredentials.GetSauceLabsUsername(),
                HubPassword = SauceLabsCredentials.GetSauceLabsAccessToken(),
                HubUrl = "http://ondemand.saucelabs.com/wd/hub"
            };
            testSettings.SeleniumHubSettings = hubSettings;
            _driver = GetWebDriver.GetSauceLabsWebDriver(browserName, os, apiName, device, version, testSettings, deviceOrientation);
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
            inputtext1.SendKeys("T");
            inputtext1.SendKeys("h");
            inputtext1.SendKeys("i");
            inputtext1.SendKeys("s");
            inputtext1.SendKeys(" ");
            inputtext1.SendKeys("i");
            inputtext1.SendKeys("s");
            inputtext1.SendKeys(" ");
            inputtext1.SendKeys("a");
            inputtext1.SendKeys(" ");
            inputtext1.SendKeys("t");
            inputtext1.SendKeys("e");
            inputtext1.SendKeys("s");
            inputtext1.SendKeys("t");
            var inputpassword1 = _driver.FindElement(By.Id("input-password")) as ITestWebElement;
            inputpassword1.Focus();
            inputpassword1.SendKeys("I");
            inputpassword1.SendKeys("t");
            inputpassword1.SendKeys(" ");
            inputpassword1.MoveToCaretPosition(0);
            inputpassword1.SendKeys("p");
            inputpassword1.SendKeys("a");
            inputpassword1.SendKeys("s");
            inputpassword1.SendKeys("s");
            inputpassword1.SendKeys("w");
            inputpassword1.SendKeys("o");
            inputpassword1.SendKeys("r");
            inputpassword1.SendKeys("d");
            var inputemail1 = _driver.FindElement(By.Id("input-email")) as ITestWebElement;
            inputemail1.Focus();
            inputemail1.SendKeys("email@test.com");
            var inputsearch1 = _driver.FindElement(By.Id("input-search")) as ITestWebElement;
            inputsearch1.Focus();
            inputsearch1.SendKeys("s");
            inputsearch1.SendKeys("e");
            inputsearch1.SendKeys("a");
            inputsearch1.SendKeys("t");
            inputsearch1.SendKeys("r");
            inputsearch1.SendKeys("c");
            inputsearch1.SendKeys("h");
            var inputdatetimelocal1 = _driver.FindElement(By.Id("input-datetime-local")) as ITestWebElement;
            inputdatetimelocal1.Focus();
            var inputrange1 = _driver.FindElement(By.Id("input-range")) as ITestWebElement;
            inputrange1.Focus();
            inputrange1.SetText("9");
            var inputtime1 = _driver.FindElement(By.Id("input-time")) as ITestWebElement;
            inputtime1.Focus();
            inputtime1.SetText("01:04");
            var div9 = _driver.FindElement(By.CssSelector("#content > FORM:nth-child(1) > SECTION:nth-child(2) > FIELDSET:nth-child(2) > DIV:nth-child(12)")) as ITestWebElement;
            div9.Click();
            var inputweek1 = _driver.FindElement(By.Id("input-week")) as ITestWebElement;
            inputweek1.Focus();
            inputweek1.SetText("2015-W45");
            div9.Click();
            var select2 = _driver.FindElement(By.Id("input-select")) as ITestWebElement;
            select2.SelectFromDropDownByIndex(2);
            var select1 = _driver.FindElement(By.Id("input-select-multipe")) as ITestWebElement;
            select1.SelectFromDropDownByIndex(0);
            select1.SelectFromDropDownByIndex(0);
            var inputradio1 = _driver.FindElement(By.Id("input-radio-1")) as ITestWebElement;
            inputradio1.Focus();
            inputradio1.Click();
            var inputcheckbox2 = _driver.FindElement(By.Id("input-checkbox-2")) as ITestWebElement;
            inputcheckbox2.Click();
            var textarea1 = _driver.FindElement(By.Id("input-textarea")) as ITestWebElement;
            textarea1.Focus();
            textarea1.SendKeys("Test Area");
            inputdatetimelocal1.Focus();
            var div18 = _driver.FindElement(By.CssSelector("#content > FORM:nth-child(1) > SECTION:nth-child(2) > FIELDSET:nth-child(2) > DIV:nth-child(20)")) as ITestWebElement;
            div18.Click();
            var div20 = _driver.FindElement(By.CssSelector("#content > FORM:nth-child(1) > SECTION:nth-child(4) > DIV:nth-child(3)")) as ITestWebElement;
            div20.Click();
            _driver.SwitchTo().Frame(0); var inputtext5 = _driver.FindElement(By.CssSelector("HTML > BODY:nth-child(2) > DIV:nth-child(1) > DIV:nth-child(2) > FORM:nth-child(1) > SECTION:nth-child(2) > FIELDSET:nth-child(2) > DIV:nth-child(1) > INPUT:nth-child(2)")) as ITestWebElement;
            inputtext5.Focus();
            inputtext5.SendKeys("I");
            inputtext5.SendKeys("f");
            inputtext5.SendKeys("r");
            inputtext5.SendKeys("a");
            inputtext5.SendKeys("m");
            inputtext5.SendKeys("e");
            _driver.SwitchTo().DefaultContent(); 
            var inputbutton2 = _driver.FindElement(By.Id("window-alert")) as ITestWebElement;
            inputbutton2.Click();
            var inputbutton3 = _driver.FindElement(By.Id("window-confirm")) as ITestWebElement;
            inputbutton3.Click();
            _driver.SwitchToAlert(AlertAction.Accept);

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
```


