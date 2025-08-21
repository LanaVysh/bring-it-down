using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace BringItDownInNovelWays.PlaywrightTests
{
    [TestFixture]
    public class CrashAppTests : PageTest
    {
        [Test]
        public async Task CrashAppButton_ShouldReturn500()
        {
            // Adjust the base URL if needed
            await Page.GotoAsync("http://localhost:5000/");

            // Intercept the fetch request and assert 500 response
            var responseTask = Page.WaitForResponseAsync(r => r.Url.Contains("/") && r.Request.Method == "POST");

            // Click the Crash App button
            await Page.ClickAsync("button[name='leak']");

            var response = await responseTask;
            Assert.AreEqual(500, response.Status, "Expected a 500 Internal Server Error response.");
        }

        [Test]
        public async Task SlowMemoryLeakButton_ShouldReturnValidResponse()
        {
            await Page.GotoAsync("http://localhost:5000/");
            var responseTask = Page.WaitForResponseAsync(r => r.Url.Contains("/") && r.Request.Method == "POST");
            await Page.ClickAsync("button[name='slowleak']");
            var response = await responseTask;
            Assert.IsTrue(response.Status == 200 || response.Status >= 400, $"Expected 200 or error, got {response.Status}");
        }

        [Test]
        public async Task RampUpCPUButton_ShouldReturnValidResponse()
        {
            await Page.GotoAsync("http://localhost:5000/");
            var responseTask = Page.WaitForResponseAsync(r => r.Url.Contains("/") && r.Request.Method == "POST");
            await Page.ClickAsync("button[name='cpu']");
            var response = await responseTask;
            Assert.IsTrue(response.Status == 200 || response.Status >= 400, $"Expected 200 or error, got {response.Status}");
        }

        [Test]
        public async Task SuccessToast_ShouldAppearOnSuccess()
        {
            await Page.GotoAsync("http://localhost:5000/");
            // Intercept and mock a successful response
            await Page.RouteAsync("**/*", route =>
            {
                if (route.Request.Method == "POST")
                    route.FulfillAsync(status: 200);
                else
                    route.ContinueAsync();
            });
            await Page.ClickAsync("button[name='cpu']");
            var toast = await Page.Locator("#toast-container div").First.WaitForAsync(new() { Timeout = 3000 });
            var text = await toast.InnerTextAsync();
            Assert.IsTrue(text.Contains("actioned"), $"Expected success toast, got: {text}");
        }

        [Test]
        public async Task ErrorToast_ShouldAppearOnError()
        {
            await Page.GotoAsync("http://localhost:5000/");
            // Intercept and mock an error response
            await Page.RouteAsync("**/*", route =>
            {
                if (route.Request.Method == "POST")
                    route.FulfillAsync(status: 500);
                else
                    route.ContinueAsync();
            });
            await Page.ClickAsync("button[name='cpu']");
            var toast = await Page.Locator("#toast-container div").First.WaitForAsync(new() { Timeout = 3000 });
            var text = await toast.InnerTextAsync();
            Assert.IsTrue(text.Contains("error"), $"Expected error toast, got: {text}");
        }
    }
}
