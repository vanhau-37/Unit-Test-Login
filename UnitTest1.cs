using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest1
	{
		private IWebDriver driver;

		[TestInitialize]
		public void Setup()
		{
			// Khởi tạo ChromeDriver
			driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
		}

		[TestMethod]
		public void TestLoginCases()
		{
			string inputFile = @"TestCase/Input.txt";
			string outputFile = @"TestCase/Output.txt";
			string resultFile = @"result.txt";

			using (StreamReader inputReader = new StreamReader(inputFile))
			using (StreamReader outputReader = new StreamReader(outputFile))
			using (StreamWriter resultWriter = new StreamWriter(resultFile))
			{
				string inputLine, expectedOutput;
				int index = 0;

				while ((inputLine = inputReader.ReadLine()) != null &&
					   (expectedOutput = outputReader.ReadLine()) != null)
				{
					index++;
					string username = "";
					string password = "";
					var inputParts = inputLine.Split(',');

					if (inputParts.Length == 1)
						username = inputParts[0];
					if (inputParts.Length >= 2)
					{
						username = inputParts[0];
						password = inputParts[1];
					}

					// Truy cập trang web
					driver.Navigate().GoToUrl("https://qlht.ued.udn.vn/");

					// Tìm và nhập username, password
					IWebElement usernameField = driver.FindElement(By.Id("txt_Login_ten_dang_nhap"));
					usernameField.Clear();
					usernameField.SendKeys(username);

					IWebElement passwordField = driver.FindElement(By.Id("pw_Login_mat_khau"));
					passwordField.Clear();
					passwordField.SendKeys(password);

					// Click nút Login
					IWebElement loginButton = driver.FindElement(By.ClassName("btn-login-form"));
					loginButton.Click();
					System.Threading.Thread.Sleep(1000); // Tạm dừng để trang load

					try
					{
						IWebElement statusLogin = driver.FindElement(By.XPath("//a[text()='Thoát']"));
						// Nếu tìm thấy phần tử, nghĩa là đã đăng nhập
						if (expectedOutput == "Success!")
							resultWriter.WriteLine($"Test {index}: Username='{ValidOrUndefined(username)}' - Password='{ValidOrUndefined(password)}' -> Test case Passed!");
						else
							resultWriter.WriteLine($"Test {index}: Username='{ValidOrUndefined(username)}' - Password='{ValidOrUndefined(password)}' -> Test case Failed!");
					}
					catch (NoSuchElementException)
					{
						// Nếu không tìm thấy phần tử, nghĩa là chưa đăng nhập
						if (expectedOutput == "Login information is incorrect!")
							resultWriter.WriteLine($"Test {index}: Username='{ValidOrUndefined(username)}' - Password='{ValidOrUndefined(password)}' -> Test case Passed!");
						else
							resultWriter.WriteLine($"Test {index}: Username='{ValidOrUndefined(username)}' - Password='{ValidOrUndefined(password)}' -> Test case Failed!");
					}
				}
			}
		}

		[TestCleanup]
		public void TearDown()
		{
			// Đóng trình duyệt sau khi test hoàn tất
			driver.Quit();
		}

		private string ValidOrUndefined(string text)
		{
			return string.IsNullOrEmpty(text) ? "Undefined" : text;
		}
	}
}

