using Microsoft.Win32;

namespace Selenium.Extensions
{
    /// <summary>
    /// Manage SauceLabs Credentials
    /// </summary>
    public class SauceLabsCredentials
    {
        /// <summary>
        /// Gets the Sauce Labs Username.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetSauceLabsUsername()
        {
            var sauceLabsUserName = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MultiBrowser\SauceLabsUsername", true);
            if (sauceLabsUserName == null)
            {
                return null;
            }
            try
            {
                var username = sauceLabsUserName.GetValue(null).ToString();
                sauceLabsUserName.Close();
                return username;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
                return null;
            }

        }

        /// <summary>
        /// Sets the Sauce Labs Username.
        /// </summary>
        /// <param name="username">The username.</param>
        public static void SetSauceLabsUsername(string username)
        {
            var usernameKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MultiBrowser\SauceLabsUsername", true);
            if (usernameKey == null)
            {
                usernameKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MultiBrowser\SauceLabsUsername");
                if (usernameKey == null) return;
                usernameKey.SetValue(null, username);
                usernameKey.Close();
            }
            else
            {
                usernameKey.SetValue(null, username);
                usernameKey.Close();
            }

        }

        /// <summary>
        /// Gets the Sauce Labs access token.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetSauceLabsAccessToken()
        {
            var passwordKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MultiBrowser\SauceLabsAccessToken", true);
            if (passwordKey == null)
            {
                return null;
            }
            try
            {
                var password = passwordKey.GetValue(null).ToString();
                passwordKey.Close();
                return password;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Sets the Sauce Labs access token.
        /// </summary>
        /// <param name="token">The access token.</param>
        public static void SetSauceLabsToken(string token)
        {
            var tokenKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MultiBrowser\SauceLabsAccessToken", true);
            if (tokenKey == null)
            {
                tokenKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MultiBrowser\SauceLabsAccessToken");
                if (tokenKey == null) return;
                tokenKey.SetValue(null, token);
                tokenKey.Close();
            }
            else
            {
                tokenKey.SetValue(null, token);
                tokenKey.Close();
            }

        }

    }
}
