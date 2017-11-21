using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using Selenium.Extensions.Emulators;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.Events;
using Selenium.Extensions.Exceptions;
using Selenium.Extensions.Interfaces;
using Xunit.Abstractions;

namespace Selenium.Extensions
{
    public class WebDriverManager
    {
        public static ITestWebDriver TestWebDriver;
        public static ITestOutputHelper TestOutputHelper;
        public static int ScreenShotCounter;

        /// <summary>
        /// Gets the current assembly directory.
        /// </summary>
        /// <value>
        /// The assembly directory.
        /// </value>
        private static string AssemblyDirectory
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
        /// <param name="testOutputHelper">The test output helper.</param>
        /// <returns></returns>
        /// <exception cref="TestConfigurationException">The details you specified are invalid</exception>
        /// <exception cref="TestConfigurationException">The details you specified are invalid</exception>
        public static ITestWebDriver InitializeInstalledBrowserDriver(TestSettings testSettings, decimal browserVersion, ITestOutputHelper testOutputHelper)
        {
            ScreenShotCounter = 0;
            TestOutputHelper = testOutputHelper;
            testSettings = ValidateSavePaths(testSettings);
            switch (testSettings.DriverType)
            {
                case WebDriverType.ChromeDriver:
                    {
                        return CreateInstalledChromeDriver(testSettings, Convert.ToInt32(browserVersion));
                    }
                case WebDriverType.FirefoxDriver:
                    {
                        return CreateInstalledFirefoxDriver(testSettings, Convert.ToInt32(browserVersion));
                    }
                case WebDriverType.InternetExplorerDriver:
                    {
                        return CreateInstalledIEDriver(testSettings);
                    }
                case WebDriverType.EdgeDriver:
                    {
                        return CreateInstalledEdgeDriver(testSettings, Convert.ToInt32(browserVersion));
                    }
            }
            throw new TestConfigurationException("The details you specified are invalid");
        }

        private static ITestWebDriver CreateInstalledEdgeDriver(TestSettings testSettings, int browserVersion)
        {
            string driverLocation = GetMultiBrowserDriverBasePath();
            driverLocation = Path.Combine(driverLocation, "EdgeDrivers", browserVersion.ToString(), "MicrosoftWebDriver.exe");
            driverLocation = ValidateDriverPresentOrUnblocked(WebDriverType.EdgeDriver, driverLocation);

            testSettings.BrowserName = "Edge";
            var driverService = EdgeDriverService.CreateDefaultService(Path.GetDirectoryName(driverLocation), Path.GetFileName(driverLocation));

            var options = new EdgeOptions();
            var driver = new EdgeDriver(driverService, options, testSettings.TimeoutTimeSpan);

            if (testSettings.DeleteAllCookies)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }

            driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

            if (testSettings.MaximiseBrowser)
            {
                driver.Manage().Window.Maximize();
            }

            var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
            TestWebDriver = extendedWebDriver;
            return extendedWebDriver;
        }

        private static ITestWebDriver CreateInstalledIEDriver(TestSettings testSettings)
        {
            testSettings.BrowserName = "IE";

            string driverBasePath = GetMultiBrowserDriverBasePath();

            var driverName = "IEDrivers\\x86\\IEDriverServer.exe";
            if (Environment.Is64BitProcess)
            {
                driverName = "IEDrivers\\x64\\IEDriverServer64.exe";
            }

            string driverLocation = Path.Combine(driverBasePath, driverName);
            driverLocation = ValidateDriverPresentOrUnblocked(WebDriverType.InternetExplorerDriver, driverLocation);
            var driverService = InternetExplorerDriverService.CreateDefaultService(Path.GetDirectoryName(driverLocation), Path.GetFileName(driverLocation));
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

            driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

            if (testSettings.MaximiseBrowser)
            {
                driver.Manage().Window.Maximize();
            }

            var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
            TestWebDriver = extendedWebDriver;
            return extendedWebDriver;
        }

        private static string GetMultiBrowserDriverBasePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MultiBrowser\\Drivers\\";
        }

        private static ITestWebDriver CreateInstalledChromeDriver(TestSettings testSettings, int browserVersion)
        {
            string driverLocation = GetMultiBrowserDriverBasePath();
            driverLocation = Path.Combine(driverLocation, "ChromeDrivers", browserVersion.ToString());

            driverLocation = ValidateDriverPresentOrUnblocked(WebDriverType.ChromeDriver, driverLocation);

            testSettings.BrowserName = "Chrome";

            var driverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(driverLocation), Path.GetFileName(driverLocation));
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

            var firingDriver = AttachDriverEvents(driver);

            if (testSettings.DeleteAllCookies)
            {
                firingDriver.Manage().Cookies.DeleteAllCookies();
            }

            driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

            if (testSettings.MaximiseBrowser)
            {
                firingDriver.Manage().Window.Maximize();
            }

            var extendedWebDriver = new TestWebDriver(firingDriver, testSettings, TestOutputHelper);
            TestWebDriver = extendedWebDriver;

            return extendedWebDriver;
        }

        private static ITestWebDriver CreateInstalledFirefoxDriver(TestSettings testSettings, int browserVersion)
        {
            testSettings.BrowserName = "Firefox";

            string driverLocation = GetMultiBrowserDriverBasePath();
            driverLocation = Path.Combine(driverLocation, "FirefoxDrivers", browserVersion.ToString());

            var driverService = FirefoxDriverService.CreateDefaultService(driverLocation);

            var options = new FirefoxOptions();
            options.UseLegacyImplementation = false;

            var driver = new FirefoxDriver(driverService, options, testSettings.TimeoutTimeSpan);

            if (testSettings.DeleteAllCookies)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }

            driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

            if (testSettings.MaximiseBrowser)
            {
                driver.Manage().Window.Maximize();
            }

            var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
            TestWebDriver = extendedWebDriver;

            return extendedWebDriver;
        }

        /// <summary>
        /// Gets the web driver for standalone browsers.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <param name="browserVersion">The browser version.</param>
        /// <param name="testOutputHelper">The test output helper.</param>
        /// <returns></returns>
        public static ITestWebDriver InitializeStandaloneBrowserDriver(TestSettings testSettings, decimal browserVersion, ITestOutputHelper testOutputHelper)
        {
            ScreenShotCounter = 0;
            TestOutputHelper = testOutputHelper;
            testSettings = ValidateSavePaths(testSettings);
            switch (testSettings.DriverType)
            {
                case WebDriverType.ChromeDriver:
                {
                    string driverLocation;
                    switch (browserVersion.ToString(CultureInfo.InvariantCulture))
                    {
                        case "48":
                        case "47":
                        case "46":
                        case "45":
                        case "44":
                        case "43":
                            driverLocation =
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\MultiBrowser\\Drivers\\ChromeDrivers\\2.20\\chromedriver.exe";
                            break;
                        case "42":
                        case "41":
                        case "40":
                        case "39":
                            driverLocation =
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\MultiBrowser\\Drivers\\ChromeDrivers\\2.14\\chromedriver.exe";
                            break;
                        case "38":
                        case "37":
                        case "36":
                            driverLocation =
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\MultiBrowser\\Drivers\\ChromeDrivers\\2.11\\chromedriver.exe";
                            break;
                        case "35":
                        case "34":
                        case "33":
                            driverLocation =
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\MultiBrowser\\Drivers\\ChromeDrivers\\2.10\\chromedriver.exe";
                            break;
                        case "32":
                        case "31":
                        case "30":
                            driverLocation =
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\MultiBrowser\\Drivers\\ChromeDrivers\\2.8\\chromedriver.exe";
                            break;
                        default:
                            driverLocation = Path.Combine(AssemblyDirectory, "chromedriver.exe");
                            break;
                    }

                    ValidateDriverPresentOrUnblocked(WebDriverType.ChromeDriver, driverLocation);
                    testSettings.BrowserName = "Chrome " + browserVersion;

                    var multiBrowserExe = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var driverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(driverLocation), Path.GetFileName(driverLocation));

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

                    driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }

                    var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
                    TestWebDriver = extendedWebDriver;

                    return extendedWebDriver;
                }
                case WebDriverType.FirefoxDriver:
                {
                    testSettings.BrowserName = "Firefox " + browserVersion;
                    var multiBrowserExe = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";

                    var driverService = FirefoxDriverService.CreateDefaultService();

                    driverService.FirefoxBinaryPath = multiBrowserExe;

                    var options = new FirefoxOptions();
                    var driver = new FirefoxDriver(driverService, options, testSettings.TimeoutTimeSpan);

                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }

                    driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }

                    var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);

                    TestWebDriver = extendedWebDriver;
                    return extendedWebDriver;
                }
                case WebDriverType.InternetExplorerDriver:
                {
                    testSettings.BrowserName = "IE " + browserVersion;
                    string driverLocation;
                    if (!Environment.Is64BitProcess)
                    {
                        driverLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MultiBrowser\\Drivers\\IEDrivers\\x86\\IEDriverServer.exe";
                    }
                    else
                    {
                        driverLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MultiBrowser\\Drivers\\IEDrivers\\x64\\IEDriverServer64.exe";
                    }

                    var driverService = InternetExplorerDriverService.CreateDefaultService(Path.GetDirectoryName(driverLocation), Path.GetFileName(driverLocation));

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

                    driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }

                    var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
                    TestWebDriver = extendedWebDriver;
                    return extendedWebDriver;
                }
                case WebDriverType.EdgeDriver:
                {
                    testSettings.BrowserName = "Edge " + browserVersion;
                    var driverService = EdgeDriverService.CreateDefaultService(AssemblyDirectory, "MicrosoftWebDriver.exe");
                    var options = new EdgeOptions();
                    var driver = new EdgeDriver(driverService, options, testSettings.TimeoutTimeSpan);

                    if (testSettings.DeleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }

                    driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;

                    if (testSettings.MaximiseBrowser)
                    {
                        driver.Manage().Window.Maximize();
                    }

                    var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);

                    TestWebDriver = extendedWebDriver;
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
        /// <param name="testOutputHelper">The test output helper.</param>
        /// <returns></returns>
        public static ITestWebDriver InitializeMultiBrowserEmulatorDriver(TestSettings testSettings, Emulator emulator, DeviceOrientation orientation, ITestOutputHelper testOutputHelper)
        {
            ScreenShotCounter = 0;
            TestOutputHelper = testOutputHelper;
            testSettings.BrowserName = emulator + " " + orientation;
            testSettings = ValidateSavePaths(testSettings);
            //string driverLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
            //                        "\\MultiBrowser\\Drivers\\ChromeDrivers\\2.20\\chromedriver.exe";
            string driverLocation = Path.Combine(AssemblyDirectory, "chromedriver.exe");
            driverLocation = ValidateDriverPresentOrUnblocked(WebDriverType.ChromeDriver, driverLocation);

            var driverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(driverLocation),
                Path.GetFileName(driverLocation));
            ValidateDriverPresentOrUnblocked(WebDriverType.ChromeDriver, driverLocation);

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

#if DEBUG
            installPathValue = @"C:\Projects\MobileEmulator\bin\Debug\x64\";
#endif
            var options = new ChromeOptions
            {
                LeaveBrowserRunning = false,
                BinaryLocation =
                    Path.Combine(installPathValue ?? @"C:\Program Files (x86)\MultiBrowser", "MultiBrowser Emulator.exe")
            };

            var emulatorSettings = MultiBrowser.GetMultiBrowserEmulators(emulator);
            if (orientation == DeviceOrientation.Portrait)
            {
                var mobileEmulationSettings = new ChromeMobileEmulationDeviceSettings
                {
                    UserAgent = emulatorSettings.DeviceUserAgent,
                    Width = emulatorSettings.DeviceWidth,
                    Height = emulatorSettings.DeviceHeight,
                    EnableTouchEvents = true,
                    PixelRatio = emulatorSettings.DevicePixelRatio
                };
                options.EnableMobileEmulation(mobileEmulationSettings);
                //options.AddAdditionalCapability("mobileEmulation", new
                //{
                //    deviceMetrics = new
                //    {
                //        width = emulatorSettings.DeviceWidth,
                //        height = emulatorSettings.DeviceHeight,
                //        pixelRatio = emulatorSettings.DevicePixelRatio
                //    },
                //    userAgent = emulatorSettings.DeviceUserAgent
                //});
            }
            else
            {
                var mobileEmulationSettings = new ChromeMobileEmulationDeviceSettings
                {
                    UserAgent = emulatorSettings.DeviceUserAgent,
                    Width = emulatorSettings.DeviceHeight,
                    Height = emulatorSettings.DeviceWidth,
                    EnableTouchEvents = true,
                    PixelRatio = emulatorSettings.DevicePixelRatio
                };
                options.EnableMobileEmulation(mobileEmulationSettings);
                

                //options.AddAdditionalCapability("mobileEmulation", new
                //{
                //    deviceMetrics = new
                //    {
                //        width = emulatorSettings.DeviceHeight,
                //        height = emulatorSettings.DeviceWidth,
                //        pixelRatio = emulatorSettings.DevicePixelRatio
                //    },
                //    userAgent = emulatorSettings.DeviceUserAgent
                //});
            }
#if DEBUG
            options.BinaryLocation = @"C:\Projects\MobileEmulator\bin\Debug\x64\MultiBrowser Emulator.exe";
#endif
            string authServerWhitelist = "auth-server-whitelist=" + testSettings.TestUri.Authority.Replace("www", "*");
            string startUrl = "startUrl=" + testSettings.TestUri.AbsoluteUri;
            string selectedEmulator = "emulator=" + emulatorSettings.EmulatorArgument;

            var argsToPass = new[]
            {
                "test-type", "start-maximized", "no-default-browser-check", "allow-no-sandbox-job",
                "disable-component-update", "disable-translate", "disable-hang-monitor", authServerWhitelist, startUrl,
                selectedEmulator
            };
            options.AddArguments(argsToPass);
            var driver = new ChromeDriver(driverService, options, testSettings.TimeoutTimeSpan);
            if (testSettings.DeleteAllCookies)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }
            driver.Manage().Timeouts().ImplicitWait = testSettings.TimeoutTimeSpan;
            var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
            TestWebDriver = extendedWebDriver;
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
        /// <param name="testOutputHelper">The test output helper.</param>
        /// <returns></returns>
        /// <exception cref="TestConfigurationException">Selenium settings not set.</exception>
        public static ITestWebDriver InitializeSauceLabsDriver(string browserName, string os, string apiName,
            string device, string version, TestSettings testSettings, DeviceOrientation deviceOrientation,
            ITestOutputHelper testOutputHelper)
        {
            ScreenShotCounter = 0;
            TestOutputHelper = testOutputHelper;
            testSettings.BrowserName = browserName;
            if (testSettings.SeleniumHubSettings == null)
            {
                throw new TestConfigurationException("SauceLabs settings not set.");
            }
            if (testSettings.SeleniumHubSettings.HubUsername == null)
            {
                throw new TestConfigurationException("SauceLabs username settings not set.");
            }
            if (testSettings.SeleniumHubSettings.HubPassword == null)
            {
                throw new TestConfigurationException("SauceLabs access token settings not set.");
            }
            var capabilities = SauceLabs.GetDesiredCapability(testSettings.SeleniumHubSettings.HubUsername,
                testSettings.SeleniumHubSettings.HubPassword, browserName, os, apiName, device, version,
                deviceOrientation, testSettings);
            testSettings = ValidateSavePaths(testSettings);

            var driver = new TestRemoteWebDriver(new Uri(testSettings.SeleniumHubSettings.HubUrl), capabilities,
                testSettings.TimeoutTimeSpan);
            var extendedWebDriver = new TestWebDriver(driver, testSettings, TestOutputHelper);
            TestWebDriver = extendedWebDriver;
            return extendedWebDriver;
        }

        /// <summary>
        /// Validates the save path.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        private static TestSettings ValidateSavePaths(TestSettings testSettings)
        {
            if (testSettings.LogScreenShots || testSettings.LogLevel != LogLevel.None)
            {
                if (!string.IsNullOrEmpty(testSettings.TestDirectory))
                {
                    if (!testSettings.TestDirectory.EndsWith("\\"))
                    {
                        testSettings.TestDirectory = testSettings.TestDirectory + "\\";
                    }
                    try
                    {
                        if (!Directory.Exists(testSettings.TestDirectory))
                        {
                            Directory.CreateDirectory(testSettings.TestDirectory);
                        }
                        if (testSettings.LogScreenShots)
                        {
                            if (!Directory.Exists(testSettings.TestDirectory + "ScreenShots"))
                            {
                                Directory.CreateDirectory(testSettings.TestDirectory + "ScreenShots");
                            }
                        }
                        if (testSettings.LogLevel != LogLevel.None)
                        {
                            if (!Directory.Exists(testSettings.TestDirectory + "Logs"))
                            {
                                Directory.CreateDirectory(testSettings.TestDirectory + "Logs");
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

        private static IWebDriver AttachDriverEvents(IWebDriver driver)
        {
            var firingDriver = new EventFiringWebDriver(driver);
            firingDriver.ExceptionThrown += ExceptionThrown;
            firingDriver.Navigated += Navigated;
            firingDriver.NavigatedBack += NavigatedBack;
            firingDriver.NavigatedForward += NavigatedForward;
            firingDriver.FindingElement += FindingElement;
            firingDriver.FindElementCompleted += FindElementCompleted;
            return firingDriver;
        }

        private static void NavigatedForward(object sender, WebDriverNavigationEventArgs e)
        {
            TestWebDriver.LogMessage(LogLevel.Verbose, $"Navigating forward to [{e.Url}]");
        }

        private static void NavigatedBack(object sender, WebDriverNavigationEventArgs e)
        {
            TestWebDriver.LogMessage(LogLevel.Verbose, $"Navigating back to [{e.Url}]");
        }

        private static void Navigated(object sender, WebDriverNavigationEventArgs e)
        {
            TestWebDriver.LogMessage(LogLevel.Verbose, $"Navigating to [{e.Url}]");
        }

        private static void FindingElement(object sender, FindElementEventArgs e)
        {
            if (e.Element == null)
            {
                TestWebDriver.LogMessage(LogLevel.Verbose, "Finding element from WebDriver: " + e.FindMethod);
            }
            else
            {
                TestWebDriver.LogMessage(LogLevel.Verbose, "Finding element from WebElement: " + e.FindMethod);
            }

        }

        private static void FindElementCompleted(object sender, FindElementEventArgs e)
        {
            if (e.Element == null)
            {
                TestWebDriver.LogMessage(LogLevel.Verbose, "Found element from WebDriver: " + e.FindMethod);
            }
            else
            {
                TestWebDriver.LogMessage(LogLevel.Verbose, "Found element from WebElement: " + e.FindMethod);
            }
            if (!TestWebDriver.Settings.HighlightElements) return;
            if (TestWebDriver.PreviousElement != null)
            {
                TestWebDriver.Highlight(TestWebDriver.PreviousElement, true);
            }
            TestWebDriver.Highlight(TestWebDriver.CurrentElement);
        }


        private static void ExceptionThrown(object sender, WebDriverExceptionEventArgs e)
        {
            TestWebDriver.LogMessage(LogLevel.Verbose, $"Exception: [{e.ThrownException.Message}]");
        }


        public static string ValidateDriverPresentOrUnblocked(WebDriverType webDriverType, string driverLocation)
        {
            if (!File.Exists(driverLocation))
            {
                //The driver doesnt exist. Lets default to the one we may have installed.
                if (webDriverType == WebDriverType.ChromeDriver)
                {
                    var chromeDrivers = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                        "\\MultiBrowser\\Drivers\\ChromeDrivers\\";
                    if (Directory.Exists(chromeDrivers))
                    {
                        var driverFolders = Directory.GetDirectories(chromeDrivers);
                        List<Version> driverVersions = new List<Version>();
                        foreach (var driverFolder in driverFolders)
                        {
                            try
                            {
                                string driverValue = driverFolder.Replace(chromeDrivers, "");
                                if (driverValue.Contains("."))
                                {
                                    string[] versionValues = driverValue.Split('.');
                                    if (versionValues.Count() > 1)
                                    {
                                        Version driverVersion = new Version(Convert.ToInt32(versionValues[0]),
                                            Convert.ToInt32(versionValues[1]));
                                        driverVersions.Add(driverVersion);
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        var finalVersions = driverVersions.OrderByDescending(d => d);
                        var folder = finalVersions.FirstOrDefault();
                        var possibleDriver = driverLocation + folder + "\\chromedriver.exe";
                        if (File.Exists(possibleDriver))
                        {
                            driverLocation = possibleDriver;
                        }
                    }
                }
                else if (webDriverType == WebDriverType.InternetExplorerDriver)
                {
                    var ieDriverPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                       "\\MultiBrowser\\Drivers\\IEDrivers\\x86\\IEDriverServer.exe";
                    if (Environment.Is64BitProcess)
                    {
                        ieDriverPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                       "\\MultiBrowser\\Drivers\\IEDrivers\\x64\\IEDriverServer64.exe";
                    }
                    if (File.Exists(ieDriverPath))
                    {
                        driverLocation = ieDriverPath;
                    }
                }
                else if (webDriverType == WebDriverType.EdgeDriver)
                {
                    var edgeDriverPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                         "\\MultiBrowser\\Drivers\\MicrosoftWebDriver\\MicrosoftWebDriver.exe";
                    if (File.Exists(edgeDriverPath))
                    {
                        driverLocation = edgeDriverPath;
                    }
                }
                //If we couldnt fix it, throw exception
                if (!File.Exists(driverLocation))
                {
                    throw new DriverNotFoundException("The driver " + driverLocation + " could not be found");
                }
            }
            if (Zone.CreateFromUrl(driverLocation).SecurityZone != SecurityZone.MyComputer)
            {
                throw new DriverBlockedException("The driver " + driverLocation +
                                                 " is blocked. Please right click the specified file and click Unblock");
            }
            return driverLocation;
        }
    }
}
