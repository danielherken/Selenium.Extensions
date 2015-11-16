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
    public class TestBuilder
    {
        public static IWebDriver GetInstalledWebDriver(string testUrl, WebDriver webDriver, int timeoutSeconds,
            bool deleteAllCookies = true, bool maximiseWindow = true)
        {
            switch (webDriver)
            {
                case WebDriver.ChromeDriver:
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
                    options.AddArgument("--auth-server-whitelist=" + new Uri(testUrl).Authority.Replace("www", "*"));
                    var driver = new ChromeDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.FirefoxDriver:
                {
                    var driverService = FirefoxDriverService.CreateDefaultService();
                    var options = new FirefoxOptions();
                    var driver = new FirefoxDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.InternetExplorerDriver:
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
                        BrowserAttachTimeout = new TimeSpan(0, 0, 0, timeoutSeconds),
                        RequireWindowFocus = true,
                        ElementScrollBehavior = InternetExplorerElementScrollBehavior.Bottom,
                        InitialBrowserUrl = testUrl,
                        EnsureCleanSession = true,
                        EnableNativeEvents = true,
                    };
                    var driver = new InternetExplorerDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.EdgeDriver:
                {
                    var driverService = EdgeDriverService.CreateDefaultService(AssemblyDirectory,
                        "MicrosoftWebDriver.exe");
                    var options = new EdgeOptions
                    {
                        PageLoadStrategy = EdgePageLoadStrategy.Default
                    };
                    var driver = new EdgeDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.OperaDriver:
                {
                    var driverService = OperaDriverService.CreateDefaultService();
                    var options = new OperaOptions
                    {
                        LeaveBrowserRunning = false
                    };
                    var driver = new OperaDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.SafariDriver:
                {
                    var options = new SafariOptions();
                    var driver = new SafariDriver(options);
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
            }
            throw new TestException("The details you specified are invalid");
        }

        public static IWebDriver GetStandaloneWebDriver(string testUrl, WebDriver webDriver, int browserVersion,
            int timeoutSeconds, bool deleteAllCookies = true, bool maximiseWindow = true)
        {
            switch (webDriver)
            {
                case WebDriver.ChromeDriver:
                {
                    string multiBrowserExe =
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
                    options.AddArgument("--auth-server-whitelist=" + new Uri(testUrl).Authority.Replace("www", "*"));
                    var driver = new ChromeDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.FirefoxDriver:
                {
                    string multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var driverService = FirefoxDriverService.CreateDefaultService();
                    driverService.FirefoxBinaryPath = multiBrowserExe;
                    var options = new FirefoxOptions();
                    var driver = new FirefoxDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.InternetExplorerDriver:
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
                        BrowserAttachTimeout = new TimeSpan(0, 0, 0, timeoutSeconds),
                        RequireWindowFocus = true,
                        ElementScrollBehavior = InternetExplorerElementScrollBehavior.Bottom,
                        InitialBrowserUrl = testUrl,
                        EnsureCleanSession = true,
                        EnableNativeEvents = true,
                    };
                    var driver = new InternetExplorerDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.EdgeDriver:
                {
                    var driverService = EdgeDriverService.CreateDefaultService(AssemblyDirectory,
                        "MicrosoftWebDriver.exe");
                    var options = new EdgeOptions
                    {
                        PageLoadStrategy = EdgePageLoadStrategy.Default
                    };
                    var driver = new EdgeDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.OperaDriver:
                {
                    string multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var driverService = OperaDriverService.CreateDefaultService();
                    var options = new OperaOptions
                    {
                        LeaveBrowserRunning = false,
                        BinaryLocation = multiBrowserExe
                    };
                    var driver = new OperaDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
                case WebDriver.SafariDriver:
                {
                    string multiBrowserExe =
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                        "\\MultiBrowser\\MB_Chrome" + browserVersion + ".exe";
                    var options = new SafariOptions
                    {
                        SafariLocation = multiBrowserExe
                    };
                    var driver = new SafariDriver(options);
                    if (deleteAllCookies)
                    {
                        driver.Manage().Cookies.DeleteAllCookies();
                    }
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
                    if (maximiseWindow)
                    {
                        driver.Manage().Window.Maximize();
                    }
                    return driver;
                }
            }
            return null;
        }

        public static IWebDriver GetMultiBrowserEmulatorWebDriver(string testUrl, Emulator emulator, int timeoutSeconds,
            bool deleteAllCookies = true)
        {
            var driverService = ChromeDriverService.CreateDefaultService(AssemblyDirectory, "chromedriver.exe");
            RegistryKey currentInstallPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MultiBrowser", false) ??
                                             Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432node\MultiBrowser",
                                                 false);
            String installPathValue = null;
            if (currentInstallPath != null)
            {
                installPathValue = (String) currentInstallPath.GetValue("Path");
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
            var driver = new ChromeDriver(driverService, options, new TimeSpan(0, 0, timeoutSeconds));
            if (deleteAllCookies)
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutSeconds));
            return driver;
        }

        public static IWebDriver GetSauceLabsWebDriver(string testUrl, string hubUrl, string sauceLabsUsername,
            string sauceLabsAccessToken, CloudBrowserVendor browserVendor, CloudBrowserName cloudBrowserName,
            int browserVersion, bool takeScreenshots, int timeoutSeconds)
        {
            DesiredCapabilities capabilities = new DesiredCapabilities();
            switch (browserVendor)
            {
                case CloudBrowserVendor.iPad:
                    capabilities = DesiredCapabilities.IPad();
                    var iPadCapabilites = new Dictionary<string, object>
                    {
                        {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                        {"username", sauceLabsUsername},
                        {"accessKey", sauceLabsAccessToken},
                        {
                            "name",
                            "iPad " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                            DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                        },
                        {"javascriptEnabled", true},
                        {"acceptSslCerts", true},
                        {"takesScreenshot", takeScreenshots},
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
                        {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                        {"username", sauceLabsUsername},
                        {"accessKey", sauceLabsAccessToken},
                        {
                            "name",
                            "iPhone " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                            DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                        },
                        {"javascriptEnabled", true},
                        {"acceptSslCerts", true},
                        {"takesScreenshot", takeScreenshots},
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
                        {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                        {"username", sauceLabsUsername},
                        {"accessKey", sauceLabsAccessToken},
                        {
                            "name",
                            "Android Emulator - " + DateTime.Now.ToShortDateString() + " " +
                            DateTime.Now.ToLongTimeString()
                        },
                        {"javascriptEnabled", true},
                        {"acceptSslCerts", true},
                        {"takesScreenshot", takeScreenshots},
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Yosemite Chrome " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Yosemite Firefox " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Yosemite Safari " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Mavericks Chrome " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Mavericks Firefox " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Mavericks Safari " + browserVersion.ToString(CultureInfo.InvariantCulture) + " - " +
                                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Mountain Lion Chrome " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Mountain Lion Firefox " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Mountain Lion Safari " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Snow Leopard Chrome " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Snow Leopard Firefox " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Snow Leopard Safari " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Snow Leopard Chrome " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Snow Leopard Firefox " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
                                {"version", browserVersion.ToString(CultureInfo.InvariantCulture)},
                                {"username", sauceLabsUsername},
                                {"accessKey", sauceLabsAccessToken},
                                {
                                    "name",
                                    "Snow Leopard Opera " + browserVersion.ToString(CultureInfo.InvariantCulture) +
                                    " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
                                },
                                {"javascriptEnabled", true},
                                {"acceptSslCerts", true},
                                {"takesScreenshot", takeScreenshots}
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
            return new ScreenshotRemoteWebDriver(new Uri(hubUrl), capabilities, new TimeSpan(0, 0, timeoutSeconds));
        }

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


    }
}
