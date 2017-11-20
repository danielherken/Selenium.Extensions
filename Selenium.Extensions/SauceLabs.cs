using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;
using Selenium.Extensions.Exceptions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;

namespace Selenium.Extensions
{
    public class SauceLabs
    {
        /// <summary>
        /// Gets the desired browser capability.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="accessKey">The access key.</param>
        /// <param name="browserName">Name of the browser.</param>
        /// <param name="os">The os.</param>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="device">The device.</param>
        /// <param name="version">The version.</param>
        /// <param name="orientation">The device orientation.</param>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        /// <exception cref="TestConfigurationException">Unable to set the desired capabilities</exception>
        public static DesiredCapabilities GetDesiredCapability(string username, string accessKey, string browserName, string os, string apiName, string device, string version, DeviceOrientation orientation, TestSettings testSettings)
        {
            DesiredCapabilities caps = null;
            switch (apiName)
            {
                case "iphone":
                case "ipad":
                    caps = DesiredCapabilities.IPad();
                    caps.SetCapability("browserName", apiName);
                    caps.SetCapability("platform", "OS X " + os.Replace("Mac ",""));
                    caps.SetCapability("version", version);
                    caps.SetCapability("deviceName", device);
                    caps.SetCapability("deviceOrientation", orientation == DeviceOrientation.Portrait ? "portrait" : "landscape");
                    caps.SetCapability("username", username);
                    caps.SetCapability("accessKey", accessKey);
                    caps.SetCapability("name", browserName + " - " + DateTime.Now.ToString("F"));
                    caps.SetCapability("javascriptEnabled", true);
                    caps.SetCapability("acceptSslCerts", true);
                    caps.SetCapability("takesScreenshot", testSettings.LogScreenShots);
                    return caps;
                case "chrome":
                case "firefox":
                case "internet explorer":
                case "microsoftedge":
                    switch (apiName)
                    {
                        case "chrome":
                            caps = (DesiredCapabilities)new ChromeOptions().ToCapabilities();
                            break;
                        case "firefox":
                            caps = (DesiredCapabilities) new FirefoxOptions().ToCapabilities();
                            break;
                        case "internet explorer":
                            caps = (DesiredCapabilities)new InternetExplorerOptions().ToCapabilities();
                            break;
                        case "microsoftedge":
                            caps = (DesiredCapabilities)new EdgeOptions().ToCapabilities();
                            break;
                        default:
                            caps = DesiredCapabilities.HtmlUnit();
                            break;
                    }
                    caps.SetCapability("browserName", device);
                    caps.SetCapability("platform", os);
                    caps.SetCapability("version", version);
                    caps.SetCapability("username", username);
                    caps.SetCapability("accessKey", accessKey);
                    caps.SetCapability("name", browserName + " - " + DateTime.Now.ToString("F"));
                    caps.SetCapability("javascriptEnabled", true);
                    caps.SetCapability("acceptSslCerts", true);
                    caps.SetCapability("takesScreenshot", testSettings.LogScreenShots);
                    return caps;
                case "android":
                    caps = DesiredCapabilities.Android();
                    if (device == "Android")
                    {
                        caps.SetCapability("browserName", device);
                        caps.SetCapability("platform", os);
                        caps.SetCapability("version", version);
                        caps.SetCapability("deviceName", "Android Emulator");
                    }
                    else
                    {
                        caps.SetCapability("browserName", "Android");
                        caps.SetCapability("deviceName", device);
                    }
                    caps.SetCapability("deviceOrientation", orientation == DeviceOrientation.Portrait ? "portrait" : "landscape");
                    caps.SetCapability("username", username);
                    caps.SetCapability("accessKey", accessKey);
                    caps.SetCapability("name", browserName + " - " + DateTime.Now.ToString("F"));
                    caps.SetCapability("javascriptEnabled", true);
                    caps.SetCapability("acceptSslCerts", true);
                    caps.SetCapability("takesScreenshot", testSettings.LogScreenShots);
                    return caps;
            }
            return null;
        }
    }
}

