using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace LW8_Test
{
    public class MainWindowTester
    {
        private Application _app;
        private UIA3Automation _automation;

        public MainWindowTester()
        {
            _app = Application.Launch(@"C:\Users\yomak\source\repos\IntegrationTesting\LabWork8\bin\Debug\net10.0-windows\LabWork8.exe");
            _automation = new UIA3Automation();

        }

        [Fact]
        public void Test_WrongLogin_Exception()
        {
            DoTest("123", "admin", nameof(Test_WrongLogin_Exception));
        }

        [Fact]
        public void Test_WrongPassword_Exception()
        {
            DoTest("admin", "123", nameof(Test_WrongPassword_Exception));
        }

        [Fact]
        public void Test_WrongLoginAndPassword_Exception()
        {
            DoTest("123", "123", nameof(Test_WrongLoginAndPassword_Exception));
        }

        [Fact]
        public void Test_WhiteSpaceLogin_Exception()
        {
            DoTest("", "admin", nameof(Test_WhiteSpaceLogin_Exception));
        }

        [Fact]
        public void Test_WhiteSpacePassword_Exception()
        {
            DoTest("admin", "", nameof(Test_WhiteSpacePassword_Exception));
        }

        [Fact]
        public void Test_TrueLoginAndPassword_Good()
        {
            DoTest("admin", "admin", nameof(Test_TrueLoginAndPassword_Good), "Успех");
        }

        public void TakeScreenshot(FlaUI.Core.AutomationElements.Window window, string name, string error = "")
        {
            var screenshot = window.Capture();
         
            if (!Directory.Exists($"screenshots/{name}"))
                Directory.CreateDirectory($"screenshots/{name}");

            screenshot.Save($"screenshots/{name}/{error}-screenshot.png");
        }

        private void DoTest(string login, string password, string funcName, string errorWindowName = "Ошибка")
        {
            var window = _app.GetMainWindow(_automation);
            var loginInput = window.FindFirstDescendant(cf => cf.ByAutomationId("LoginTextBox")).AsTextBox();
            var passwordInput = window.FindFirstDescendant(cf => cf.ByAutomationId("PasswordTextBox")).AsTextBox();
            var button = window.FindFirstDescendant(cf => cf.ByAutomationId("AuthButton")).AsButton();

            passwordInput.Text = password;
            loginInput.Text = login;
            button.Invoke();

            var errorWindow = _automation.GetDesktop().FindFirstDescendant(cf => cf.ByName(errorWindowName)).AsWindow();
            TakeScreenshot(errorWindow, funcName, "error-");
            errorWindow.Close();
            TakeScreenshot(window, funcName);

            Assert.NotNull(errorWindow);
            _app.Close();
            _automation.Dispose();
        }
    }
}
