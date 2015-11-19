using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace Selenium.Extensions
{
    public class GetWebDriver
    {
        /// <summary>
        /// Gets the current assembly directory.
        /// </summary>
        /// <value>
        /// The assembly directory.
        /// </value>
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Gets the web driver for locally installed browsers.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        /// <exception cref="Selenium.Extensions.TestException">The details you specified are invalid</exception>
        public static ITestWebDriver GetInstalledBrowserWebDriver(TestSettings testSettings)
        {
            testSettings = ValidateSavePath(testSettings);
            switch (testSettings.DriverType)
            {
                case WebDriverType.ChromeDriver:
                    {
                        testSettings.BrowserName = "Chrome";
                        var driverService = ChromeDriverService.CreateDefaultService(AssemblyDirectory, "chromedriver.exe");
                        var options = new ChromeOptions
                        {
                            LeaveBrowserRunning = false
                        };
                        options.AddArgument("--no-default-browser-check");
                        options.AddArgument("--test-type=browser");
                        options.AddArgument("--start-maximized");
                        options.AddArgument("--allow-no-sandbox-job");
                        options.AddArgument("--disable-component-update");
                        options.AddArgument("--auth-server-whitelist=" + testSettings.TestUri.Authority.Replace("www", "*"));
                        var driver = new ChromeDriver(driverService, options, testSettings.TimeoutTimeSpan);
                        if (testSettings.DeleteAllCookies)
                        {
                            driver.Manage().Cookies.DeleteAllCookies();
                        }
                        driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                        if (testSettings.MaximiseBrowser)
                        {
                            driver.Manage().Window.Maximize();
                        }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.FirefoxDriver:
                    {
                        testSettings.BrowserName = "Firefox";
                        var driverService = FirefoxDriverService.CreateDefaultService();
                        var options = new FirefoxOptions();
                        var driver = new FirefoxDriver(driverService, options,testSettings.TimeoutTimeSpan);
                        if (testSettings.DeleteAllCookies)
                        {
                            driver.Manage().Cookies.DeleteAllCookies();
                        }
                        driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                        if (testSettings.MaximiseBrowser)
                        {
                            driver.Manage().Window.Maximize();
                        }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.InternetExplorerDriver:
                    {
                        testSettings.BrowserName = "IE";
                        var driverName = "IEDriverServer.exe";
                        if (Environment.Is64BitProcess)
                        {
                            driverName = "IEDriverServer64.exe";
                        }
                        var driverService = InternetExplorerDriverService.CreateDefaultService(AssemblyDirectory, driverName);
                        var options = new InternetExplorerOptions
                        {
                            IgnoreZoomLevel = true,
                            IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                            BrowserAttachTimeout = testSettings.TimeoutTimeSpan,
                            RequireWindowFocus = true,
                            ElementScrollBehavior = InternetExplorerElementScrollBehavior.Bottom,
                            InitialBrowserUrl = testSettings.TestUri.AbsoluteUri,
                            EnsureCleanSession = true,
                            EnableNativeEvents = true
                        };
                        var driver = new InternetExplorerDriver(driverService, options, testSettings.TimeoutTimeSpan);
                        if (testSettings.DeleteAllCookies)
                        {
                            driver.Manage().Cookies.DeleteAllCookies();
                        }
                        driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                        if (testSettings.MaximiseBrowser)
                        {
                            driver.Manage().Window.Maximize();
                        }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.EdgeDriver:
                    {
                        testSettings.BrowserName = "Edge";
                        var driverService = EdgeDriverService.CreateDefaultService(AssemblyDirectory,
                            "MicrosoftWebDriver.exe");
                        var options = new EdgeOptions
                        {
                            PageLoadStrategy = EdgePageLoadStrategy.Default
                        };
                        var driver = new EdgeDriver(driverService, options, testSettings.TimeoutTimeSpan);
                        if (testSettings.DeleteAllCookies)
                        {
                            driver.Manage().Cookies.DeleteAllCookies();
                        }
                        driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                        if (testSettings.MaximiseBrowser)
                        {
                            driver.Manage().Window.Maximize();
                        }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.OperaDriver:
                    {
                        testSettings.BrowserName = "Opera";
                        var driverService = OperaDriverService.CreateDefaultService();
                        var options = new OperaOptions
                        {
                            LeaveBrowserRunning = false
                        };
                        var driver = new OperaDriver(driverService, options, testSettings.TimeoutTimeSpan);
                        if (testSettings.DeleteAllCookies)
                        {
                            driver.Manage().Cookies.DeleteAllCookies();
                        }
                        driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                        if (testSettings.MaximiseBrowser)
                        {
                            driver.Manage().Window.Maximize();
                        }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.SafariDriver:
                    {
                        testSettings.BrowserName = "Safari";
                        var options = new SafariOptions();
                        var driver = new SafariDriver(options);
                        if (testSettings.DeleteAllCookies)
                        {
                            driver.Manage().Cookies.DeleteAllCookies();
                        }
                        driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                        if (testSettings.MaximiseBrowser)
                        {
                            driver.Manage().Window.Maximize();
                        }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
            }
            throw new TestException("The details you specified are invalid");
        }

        /// <summary>
        /// Gets the web driver for standalone browsers.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <param name="browserVersion">The browser version.</param>
        /// <returns></returns>
        public static ITestWebDriver GetStandaloneWebDriver(TestSettings testSettings, decimal browserVersion)
        {
            testSettings = ValidateSavePath(testSettings);
            switch (testSettings.DriverType)
            {
                case WebDriverType.ChromeDriver:
                {
                    testSettings.BrowserName = "Chrome " + browserVersion;
                    var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var driverService = ChromeDriverService.CreateDefaultService(AssemblyDirectory, "chromedriver.exe");
                    var options = new ChromeOptions
                    {
                        LeaveBrowserRunning = false,
                        BinaryLocation = multiBrowserExe
                    };
                    options.AddArgument("--no-default-browser-check");
                    options.AddArgument("--test-type=browser");
                    options.AddArgument("--start-maximized");
                    options.AddArgument("--allow-no-sandbox-job");
                    options.AddArgument("--disable-component-update");
                    options.AddArgument("--auth-server-whitelist=" + testSettings.TestUri.Authority.Replace("www", "*"));
                    var driver = new ChromeDriver(driverService, options, testSettings.TimeoutTimeSpan);
                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.FirefoxDriver:
                {
                        testSettings.BrowserName = "Firefox " + browserVersion;
                        var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var driverService = FirefoxDriverService.CreateDefaultService();
                    driverService.FirefoxBinaryPath = multiBrowserExe;
                    var options = new FirefoxOptions();
                    var driver = new FirefoxDriver(driverService, options, testSettings.TimeoutTimeSpan);
                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.InternetExplorerDriver:
                {
                        testSettings.BrowserName = "IE " + browserVersion;
                        var driverName = "IEDriverServer.exe";
                    if (Environment.Is64BitProcess)
                    {
                        driverName = "IEDriverServer64.exe";
                    }
                    var driverService = InternetExplorerDriverService.CreateDefaultService(AssemblyDirectory, driverName);
                    var options = new InternetExplorerOptions
                    {
                        IgnoreZoomLevel = true,
                        IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                        BrowserAttachTimeout = testSettings.TimeoutTimeSpan,
                        RequireWindowFocus = true,
                        ElementScrollBehavior = InternetExplorerElementScrollBehavior.Bottom,
                        InitialBrowserUrl = testSettings.TestUri.AbsoluteUri,
                        EnsureCleanSession = true,
                        EnableNativeEvents = true
                    };
                    var driver = new InternetExplorerDriver(driverService, options, testSettings.TimeoutTimeSpan);
                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.EdgeDriver:
                {
                        testSettings.BrowserName = "Edge " + browserVersion;
                        var driverService = EdgeDriverService.CreateDefaultService(AssemblyDirectory,
                        "MicrosoftWebDriver.exe");
                    var options = new EdgeOptions
                    {
                        PageLoadStrategy = EdgePageLoadStrategy.Default
                    };
                    var driver = new EdgeDriver(driverService, options, testSettings.TimeoutTimeSpan);
                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.OperaDriver:
                {
                        testSettings.BrowserName = "Opera " + browserVersion;
                        var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var driverService = OperaDriverService.CreateDefaultService();
                    var options = new OperaOptions
                    {
                        LeaveBrowserRunning = false,
                        BinaryLocation = multiBrowserExe
                    };
                    var driver = new OperaDriver(driverService, options, testSettings.TimeoutTimeSpan);
                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
                case WebDriverType.SafariDriver:
                {
                        testSettings.BrowserName = "Firefox " + browserVersion;
                        var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var options = new SafariOptions
                    {
                        SafariLocation = multiBrowserExe
                    };
                    var driver = new SafariDriver(options);
                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }
                        var extendedWebDriver = new TestWebDriver(driver, testSettings);
                        return extendedWebDriver;
                    }
            }
            return null;
        }

        /// <summary>
        /// Gets the multi browser emulator web driver.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <param name="emulator">The emulator.</param>
        /// <param name="orientation">The device orientation.</param>
        /// <returns></returns>
        public static ITestWebDriver GetMultiBrowserEmulatorWebDriver(TestSettings testSettings, Emulator emulator, DeviceOrientation orientation)
        {
            testSettings.BrowserName = emulator + " " + orientation;
            testSettings = ValidateSavePath(testSettings);
            var driverService = ChromeDriverService.CreateDefaultService(AssemblyDirectory, "chromedriver.exe");
            var currentInstallPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MultiBrowser", false) ??
                                     Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432node\MultiBrowser",
                                         false);
            string installPathValue = null;
            if (currentInstallPath != null)
            {
                installPathValue = (string) currentInstallPath.GetValue("Path");
            }
            if (installPathValue != null)
            {
                if (!installPathValue.EndsWith("\\"))
                {
                    installPathValue = installPathValue + "\\";
                }
            }
            var options = new ChromeOptions
            {
                LeaveBrowserRunning = false,
                BinaryLocation =
                    Path.Combine(installPathValue ?? @"C:\Program Files (x86)\MultiBrowser", "MultiBrowser Emulator.exe")
            };
            var driver = new ChromeDriver(driverService, options, testSettings.TimeoutTimeSpan);
            if (testSettings.DeleteAllCookies)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }
            driver.Manage().Timeouts().ImplicitlyWait(testSettings.TimeoutTimeSpan);
            var extendedWebDriver = new TestWebDriver(driver, testSettings);
            return extendedWebDriver;
        }

        /// <summary>
        /// Gets the sauce labs web driver.
        /// </summary>
        /// <param name="browserName">The full browser name.</param>
        /// <param name="os">The operating system.</param>
        /// <param name="apiName">The api name.</param>
        /// <param name="device">The device name.</param>
        /// <param name="version">The version name.</param>
        /// <param name="testSettings">The test settings.</param>
        /// <param name="deviceOrientation">The device orientation.</param>
        /// <returns></returns>
        /// <exception cref="TestException">Selenium settings not set.</exception>
        public static ITestWebDriver GetSauceLabsWebDriver(string browserName, string os, string apiName, string device, string version, TestSettings testSettings, DeviceOrientation deviceOrientation)
        {
            testSettings.BrowserName = browserName;
            if (testSettings.SeleniumHubSettings == null)
            {
                throw new TestException("SauceLabs settings not set.");
            }
            if (testSettings.SeleniumHubSettings.HubUsername == null)
            {
                throw new TestException("SauceLabs username settings not set.");
            }
            if (testSettings.SeleniumHubSettings.HubPassword == null)
            {
                throw new TestException("SauceLabs access token settings not set.");
            }
            var capabilities = SauceLabs.GetDesiredCapability(testSettings.SeleniumHubSettings.HubUsername,testSettings.SeleniumHubSettings.HubPassword, browserName, os, apiName, device, version, deviceOrientation, testSettings);
            testSettings = ValidateSavePath(testSettings);

            var driver = new TestRemoteWebDriver(new Uri(testSettings.SeleniumHubSettings.HubUrl), capabilities, testSettings.TimeoutTimeSpan);
            var extendedWebDriver = new TestWebDriver(driver, testSettings);
            return extendedWebDriver;
        }

        /// <summary>
        /// Validates the save path.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        private static TestSettings ValidateSavePath(TestSettings testSettings)
        {
            if (testSettings.LogScreenShots || testSettings.LogEvents)
            {
                if (!string.IsNullOrEmpty(testSettings.TestDirectory))
                {
                    if (!testSettings.TestDirectory.EndsWith("\\"))
                    {
                        testSettings.TestDirectory = testSettings.TestDirectory + "\\";
                    }
                    try
                    {
                        var path = Path.GetFullPath(testSettings.TestDirectory);
                        if (!Directory.Exists(testSettings.TestDirectory))
                        {
                            Directory.CreateDirectory(testSettings.TestDirectory);
                        }
                        if (testSettings.LogScreenShots)
                        {
                            if (!Directory.Exists(testSettings.TestDirectory = "ScreenShots"))
                            {
                                Directory.CreateDirectory(testSettings.TestDirectory = "ScreenShots");
                            }
                        }
                        if (testSettings.LogEvents)
                        {
                            if (!Directory.Exists(testSettings.TestDirectory = "Logs"))
                            {
                                Directory.CreateDirectory(testSettings.TestDirectory = "Logs");
                            }
                        }
                    }
                    catch
                    {
                        testSettings.TestDirectory = "";
                    }

                }
            }
            return testSettings;
        }
    }
}