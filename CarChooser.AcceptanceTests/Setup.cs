using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coypu;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using TechTalk.SpecFlow;

namespace CarChooser.AcceptanceTests
{
    [Binding]
    public static class Setup
    {
        public static BrowserSession Browser;
        
        [BeforeTestRun]
        public static void SetupBrowser()
        {
            var sessionConfiguration = new SessionConfiguration
                {
                    AppHost = "localhost",
                    Port = 27021,
                    Driver = typeof (SeleniumWebDriver),
                    Browser = Coypu.Drivers.Browser.Chrome
                };
            
            Browser = new BrowserSession(sessionConfiguration);
        }

        [AfterTestRun]
        public static void CloseBrowser()
        {
            Browser.Dispose();
        }
    }
}
