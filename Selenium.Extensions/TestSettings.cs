using System;

namespace Selenium.Extensions
{

    public class TestSettings
    {

        /// <summary>
        /// Gets or sets the test URI.
        /// </summary>
        /// <value>
        /// The test URI.
        /// </value>
        public Uri TestUri { get; set; }

        /// <summary>
        /// Gets or sets the type of the driver.
        /// </summary>
        /// <value>
        /// The type of the driver.
        /// </value>
        public WebDriverType DriverType { get; set; }
        /// <summary>
        /// Gets or sets the timeout time span.
        /// </summary>
        /// <value>
        /// The timeout time span.
        /// </value>
        public TimeSpan TimeoutTimeSpan { get; set; }
        /// <summary>
        /// Gets or sets whether to delete all cookies before starting a test.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the driver should delete all cookies; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteAllCookies { get; set; }
        /// <summary>
        /// Gets or setswhether the driver should maximise the browser before starting a test.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the driver should maximise the browser; otherwise, <c>false</c>.
        /// </value>
        public bool MaximiseBrowser { get; set; }

        /// <summary>
        /// Gets or sets the screen shot directory.
        /// </summary>
        /// <value>
        /// The screen shot directory.
        /// </value>
        public string ScreenShotDirectory { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        /// <value>
        /// The browser version.
        /// </value>
        public int BrowserVersion { get; set; }


        /// <summary>
        ///     Setting for when to allow re-resolving of elements
        /// </summary>
        public PageOriginStrictness SamePageOriginStrictness { get; set; }

        /// <summary>
        /// Gets or sets the selenium hub settings.
        /// </summary>
        /// <value>
        /// The selenium hub settings.
        /// </value>
        public HubSettings SeleniumHubSettings { get; set; }

        public static TestSettings Default => new TestSettings
        {
            SamePageOriginStrictness = PageOriginStrictness.AllowNonMatchingAnchorHashes,
            TestUri = new Uri("https://www.google.com"),
            DriverType = WebDriverType.ChromeDriver,
            TimeoutTimeSpan = TimeSpan.FromSeconds(30),
            DeleteAllCookies = true,
            MaximiseBrowser = true,
            ScreenShotDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        public enum PageOriginStrictness
        {
            /// <summary>
            ///     Don't perform any check when re-resolving stale elements
            /// </summary>
            DontCheckOrigin,

            /// <summary>
            ///     Check that the uri of the page where the element was first resolved matches the page where the element is
            ///     re-resolved after becoming stale but ignore differences in #anchor part of the uri.
            /// </summary>
            AllowNonMatchingAnchorHashes,

            /// <summary>
            ///     Check that the uri of the page where the element was first resolved matches the page where the element is
            ///     re-resolved after becoming stale but ignore differences in query strings and #anchor part of the uri.
            /// </summary>
            AllowNonMatchingQueryStrings,

            /// <summary>
            ///     Check that the uri of the page where the element was first resolved matches exactly to the page where the element
            ///     is re-resolved after becoming stale
            /// </summary>
            ExactMatch
        }
    }

    public class HubSettings
    {
        /// <summary>
        /// Gets or sets the hub URL.
        /// </summary>
        /// <value>
        /// The hub URL.
        /// </value>
        public string HubUrl { get; set; }

        /// <summary>
        /// Gets or sets the hub username.
        /// </summary>
        /// <value>
        /// The hub username.
        /// </value>
        public string HubUsername { get; set; }

        /// <summary>
        /// Gets or sets the hub password.
        /// </summary>
        /// <value>
        /// The hub password.
        /// </value>
        public string HubPassword { get; set; }
    }
}