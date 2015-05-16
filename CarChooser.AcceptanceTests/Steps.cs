using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace CarChooser.AcceptanceTests
{
    [Binding]
    public class Steps
    {

        [Given(@"I have navigated to the homepage")]
        public void GivenIHaveNavigatedToTheHomepage()
        {
            Setup.Browser.Visit("Home");
        }

        [Then(@"I see an (.*)")]
        public void ShouldSeeContent(string content)
        {
            Assert.True(Setup.Browser.HasContent(content));
        }

    }
}
