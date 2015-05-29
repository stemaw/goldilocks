using System;
using Coypu;
using Coypu.Drivers.Selenium;

namespace DataImporter
{
    public class Browser
    {
        public static BrowserSession SpinUpBrowser()
        {
            var sessionConfiguration = new SessionConfiguration
            {
                AppHost = "localhost",
                Port = 27021,
                Driver = typeof(SeleniumWebDriver),
                Browser = Coypu.Drivers.Browser.Chrome,
                Timeout = TimeSpan.FromSeconds(30),
                RetryInterval = TimeSpan.FromMilliseconds(2)
            };

            return new BrowserSession(sessionConfiguration);
        }
    }
}
