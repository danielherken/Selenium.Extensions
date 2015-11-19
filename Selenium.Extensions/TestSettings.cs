using System;
using OpenQA.Selenium.Remote;

namespace Selenium.Extensions
{

    /// <summary>
    /// The test settings for the test
    /// </summary>
    public class TestSettings
    {
        /// <summary>
        /// Gets or sets the test type.
        /// </summary>
        /// <value>
        /// The type of test.
        /// </value>
        public TestType TestType { get; set; }

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
        /// Gets or sets the directory to save logs and screen shots to.
        /// </summary>
        /// <value>
        /// The test directory.
        /// </value>
        public string TestDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to take screen shots.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the test takes screen shots; otherwise, <c>false</c>.
        /// </value>
        public bool LogScreenShots { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create a test log].
        /// </summary>
        /// <value>
        ///   <c>true</c> if the test should create a log, otherwise <c>false</c>.
        /// </value>
        public bool LogEvents { get; set; }

        /// <summary>
        /// Gets or sets the name of the browser.
        /// </summary>
        /// <value>
        /// The name of the browser.
        /// </value>
        public string BrowserName { get; set; }

        /// <summary>
        /// Gets or sets whether to highlight elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the test should highlight the active elements; otherwise, <c>false</c>.
        /// </value>
        public bool HighlightElements { get; set; }

        public bool CapturePerformance { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        public LogLevel LogLevel { get; set; }


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

        /// <summary>
        /// Gets or sets the default test settings.
        /// </summary>
        /// <value>
        /// The default test settings.
        /// </value>
        public static TestSettings Default => new TestSettings
        {
            SamePageOriginStrictness = PageOriginStrictness.AllowNonMatchingAnchorHashes,
            TestType = TestType.InstalledBrowser,
            TestUri = new Uri("https://www.google.com"),
            DriverType = WebDriverType.ChromeDriver,
            TimeoutTimeSpan = TimeSpan.FromSeconds(30),
            DeleteAllCookies = true,
            MaximiseBrowser = true,
            LogScreenShots = true,
            LogEvents = false,
            LogLevel = LogLevel.None,
            HighlightElements = false,
            TestDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),

        };

        /// <summary>
        /// Gets the Page Origin strictness rule
        /// </summary>
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

    /// <summary>
    /// The hub settings for cloud testing
    /// </summary>
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