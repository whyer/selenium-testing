using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Annotations;

namespace SeleniumTests
{
	[TestFixture]
    public class BasicTests
    {
		private IWebDriver _driver;

		[SetUp]
		public void RunBeforeEachTest()
		{
			_driver = new FirefoxDriver();
			//driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
			//driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
		}

		[TearDown]
		public void RunAfterEachTest()
		{
			if (TestContext.CurrentContext.Result.Status == TestStatus.Failed)
			{
				_driver.TakeScreenshot().SaveAsFile("c:\\temp\\fail.png", ImageFormat.Png);
			}

			Thread.Sleep(TimeSpan.FromSeconds(4));
			_driver.Quit();
		}

		[Test]
		public void GoogleSearch()
		{
			GoogleSearchPage searchPage = GoogleSearchPage.NavigateToPage(_driver);

			GoogleResultPage resultPage = searchPage.DoSearch("Kalle Anka");
			Assert.That(resultPage.Title, Is.StringStarting("Kalle Anka"));
		}
    }

	public class GoogleSearchPage
	{
		private readonly IWebDriver _driver;
		public GoogleSearchPage(IWebDriver driver)
		{
			_driver = driver;
		}

		public static GoogleSearchPage NavigateToPage(IWebDriver driver)
		{
			driver.Navigate().GoToUrl("http://www.google.com");
			var searchPage = PageFactory.InitElements<GoogleSearchPage>(driver);
			return searchPage;
		}

		[FindsBy(How = How.Name, Using = "q"), UsedImplicitly] 
		private IWebElement _searchField;

		[FindsBy(How = How.Name, Using = "btnG"), UsedImplicitly]
		private IWebElement _searchButton;

		public GoogleResultPage DoSearch(string searchQuery)
		{
			_searchField.SendKeys(searchQuery);
			_searchButton.Click();

			return new GoogleResultPage(_driver, searchQuery);
		}
	}

	public class GoogleResultPage
	{
		private readonly IWebDriver _driver;
		private readonly string _searchQuery;

		public GoogleResultPage(IWebDriver driver, string searchQuery)
		{
			if (driver == null) throw new ArgumentNullException("driver");
			if (searchQuery == null) throw new ArgumentNullException("searchQuery");
			_driver = driver;
			_searchQuery = searchQuery;
		}

		public string Title
		{
			get
			{
				WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
				wait.Until((d) => d.Title.ToLower().StartsWith(_searchQuery.ToLower()));
				return _driver.Title;
			}
		}
	}
}
