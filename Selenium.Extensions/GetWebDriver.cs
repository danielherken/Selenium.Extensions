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
            if (testSettings.ScreenShotDirectory != null)
            {
                if (!Directory.Exists(testSettings.ScreenShotDirectory))
                {
                    Directory.CreateDirectory(testSettings.ScreenShotDirectory);
                }
            }
            switch (testSettings.DriverType)
            {
                case WebDriverType.ChromeDriver:
                    {
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
        /// <returns></returns>
        public static ITestWebDriver GetStandaloneWebDriver(TestSettings testSettings)
        {
            switch (testSettings.DriverType)
            {
                case WebDriverType.ChromeDriver:
                {
                    var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + testSettings.BrowserVersion + ".exe";
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
                    var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + testSettings.BrowserVersion + ".exe";
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
                    var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + testSettings.BrowserVersion + ".exe";
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
                    var multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + testSettings.BrowserVersion + ".exe";
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
        /// <returns></returns>
        public static ITestWebDriver GetMultiBrowserEmulatorWebDriver(TestSettings testSettings, Emulator emulator)
        {
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
        /// <param name="testSettings">The test settings.</param>
        /// <param name="browserVendor">The browser vendor.</param>
        /// <param name="cloudBrowserName">Name of the cloud browser.</param>
        /// <returns></returns>
        public static ITestWebDriver GetSauceLabsWebDriver(TestSettings testSettings, CloudBrowserVendor browserVendor, CloudBrowserName cloudBrowserName)
        {
            var capabilities = new DesiredCapabilities();
            switch (browserVendor)
            {
                case CloudBrowserVendor.iPad:
                    capabilities = DesiredCapabilities.IPad();
                    var iPadCapabilites = new Dictionary<string, object>
                    {
                        {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                        {"username", testSettings.SeleniumHubSettings.HubUsername},
                        {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                        {
                            "name",
                            "iPad " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                            DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                        },
                        {"javascriptEnabled", true},
                        {"acceptSslCerts", true},
                        {"takesScreenshot", true},
                        {"device-orientation", "portrait"}
                    };
                    foreach (var capability in iPadCapabilites)
                    {
                        capabilities.SetCapability(capability.Key, capability.Value);
                    }
                    break;
                case CloudBrowserVendor.iPhone:
                    capabilities = DesiredCapabilities.IPhone();
                    var iPhoneCapabilites = new Dictionary<string, object>
                    {
                        {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                        {"username", testSettings.SeleniumHubSettings.HubUsername},
                        {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                        {
                            "name",
                            "iPhone " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                            DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                        },
                        {"javascriptEnabled", true},
                        {"acceptSslCerts", true},
                        {"takesScreenshot", true},
                        {"device-orientation", "portrait"}
                    };
                    foreach (var capability in iPhoneCapabilites)
                    {
                        capabilities.SetCapability(capability.Key, capability.Value);
                    }
                    break;
                case CloudBrowserVendor.Android:
                    capabilities = DesiredCapabilities.Android();
                    var androidCapabilites = new Dictionary<string, object>
                    {
                        {"platform", "Linux"},
                        {"deviceName", "Android Emulator"},
                        {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                        {"username", testSettings.SeleniumHubSettings.HubUsername},
                        {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                        {
                            "name",
                            "Android Emulator - " + DateTime.Now.ToShortDateString() + " " +
                            DateTime.Now.ToLongTimeString()
                        },
                        {"javascriptEnabled", true},
                        {"acceptSslCerts", true},
                        {"takesScreenshot", true},
                        {"device-orientation", "portrait"}
                    };
                    foreach (var capability in androidCapabilites)
                    {
                        capabilities.SetCapability(capability.Key, capability.Value);
                    }
                    break;
                case CloudBrowserVendor.Yosemite:
                    switch (cloudBrowserName)
                    {
                        case CloudBrowserName.Chrome:
                        {
                            capabilities = DesiredCapabilities.Chrome();
                            var yosemiteChromeCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.10"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Yosemite Chrome " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in yosemiteChromeCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Firefox:
                        {
                            capabilities = DesiredCapabilities.Firefox();
                            var yosemiteFirefoxCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.10"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Yosemite Firefox " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in yosemiteFirefoxCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Safari:
                        {
                            capabilities = DesiredCapabilities.Safari();
                            var yosemiteSafariCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.10"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Yosemite Safari " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in yosemiteSafariCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                    }
                    break;
                case CloudBrowserVendor.Mavericks:
                    switch (cloudBrowserName)
                    {
                        case CloudBrowserName.Chrome:
                        {
                            capabilities = DesiredCapabilities.Chrome();
                            var maveriksChromeCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.9"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Mavericks Chrome " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in maveriksChromeCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Firefox:
                        {
                            capabilities = DesiredCapabilities.Firefox();
                            var maveriksFirefoxCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.9"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Mavericks Firefox " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in maveriksFirefoxCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Safari:
                        {
                            capabilities = DesiredCapabilities.Safari();
                            var maveriksSafariCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.9"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Mavericks Safari " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in maveriksSafariCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                    }
                    break;
                case CloudBrowserVendor.MountainLion:
                    switch (cloudBrowserName)
                    {
                        case CloudBrowserName.Chrome:
                        {
                            capabilities = DesiredCapabilities.Chrome();
                            var mountainLionChromeCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.8"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Mountain Lion Chrome " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in mountainLionChromeCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Firefox:
                        {
                            capabilities = DesiredCapabilities.Firefox();
                            var mountainLionFirefoxCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.8"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Mountain Lion Firefox " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in mountainLionFirefoxCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Safari:
                        {
                            capabilities = DesiredCapabilities.Safari();
                            var mountainLionSafariCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.8"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Mountain Lion Safari " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in mountainLionSafariCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                    }
                    break;
                case CloudBrowserVendor.SnowLeopard:
                    switch (cloudBrowserName)
                    {
                        case CloudBrowserName.Chrome:
                        {
                            capabilities = DesiredCapabilities.Chrome();
                            var snowLeopardChromeCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.6"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Snow Leopard Chrome " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in snowLeopardChromeCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Firefox:
                        {
                            capabilities = DesiredCapabilities.Firefox();
                            var snowLeopardFirefoxCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.6"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Snow Leopard Firefox " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in snowLeopardFirefoxCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Safari:
                        {
                            capabilities = DesiredCapabilities.Safari();
                            var snowLeopardSafariCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "OS X 10.6"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Snow Leopard Safari " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in snowLeopardSafariCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                    }
                    break;
                case CloudBrowserVendor.Linux:
                    switch (cloudBrowserName)
                    {
                        case CloudBrowserName.Chrome:
                        {
                            capabilities = DesiredCapabilities.Chrome();
                            var linuxChromeCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "Linux"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Snow Leopard Chrome " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in linuxChromeCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Firefox:
                        {
                            capabilities = DesiredCapabilities.Firefox();
                            var linuxFirefoxCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "Linux"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Snow Leopard Firefox " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in linuxFirefoxCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                        case CloudBrowserName.Opera:
                        {
                            capabilities = DesiredCapabilities.Opera();
                            var linuxOperaCapabilites = new Dictionary<string, object>
                            {
                                {"platform", "Linux"},
                                {"version", testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", testSettings.SeleniumHubSettings.HubUsername},
                                {"accessKey", testSettings.SeleniumHubSettings.HubPassword},
                                {
                                    "name",
                                    "Snow Leopard Opera " + testSettings.BrowserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", true}
                            };
                            foreach (var capability in linuxOperaCapabilites)
                            {
                                capabilities.SetCapability(capability.Key, capability.Value);
                            }
                        }
                            break;
                    }
                    break;
            }
            var driver = new ScreenshotRemoteWebDriver(new Uri(testSettings.SeleniumHubSettings.HubUrl), capabilities, testSettings.TimeoutTimeSpan);
            var extendedWebDriver = new TestWebDriver(driver, testSettings);
            return extendedWebDriver;
        }
    }
}