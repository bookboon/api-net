using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BookboonSDK.Tests
{
    public class BookboonClientTests
    {
        [Test]
        public async void SimpleGet()
        {
            var client = new BookboonClient();
            
            var data = await client.Get("/categories");

            Assert.NotNull(data[0].id);
        }

        [Test]
        public async void AuthenticatedGet()
        {
            var client = new BookboonClient();
            var handle = GetAuthenticationHandle();

            var data = await client.Get("/recommendations", handle);

            Assert.NotNull(data[0].id);
        }

        [Test]
        public async void AuthenticatedPost()
        {
            var client = new BookboonClient();
            var handle = GetAuthenticationHandle();

            var data = await client.Post("/profile", handle, new { email = "test@example.com", newsletter = false });

            Assert.AreEqual("test@example.com", (string) data.email);
        }

        [Test]
        public async void NotFoundException()
        {
            var client = new BookboonClient();

            var task = client.Get("/nosuchthing");

            AssertThrowsApiException(HttpStatusCode.NotFound, task);
        }

        [Test]
        public async void HttpsRequiredException()
        {
            var client = new BookboonClient();

            var task = client.Get("/recommendations");

            AssertThrowsApiException(HttpStatusCode.Forbidden, task);
        }

        [Test]
        public async void ApiKeyInvalidException()
        {
            var client = new BookboonClient();
            var handle = new AuthenticationHandle("badapikey", "dummyhandle");

            var task = client.Get("/recommendations", handle);

            AssertThrowsApiException(HttpStatusCode.Unauthorized, task);
        }

        [Test]
        public async void BadDataException()
        {
            var client = new BookboonClient();
            var handle = GetAuthenticationHandle();

            var task = client.Post("/profile", handle, new { email = "invalid", newsletter = false });

            AssertThrowsApiException(HttpStatusCode.BadRequest, task);
        }

        public static async void AssertThrowsApiException(HttpStatusCode expectedHttpStatusCode, Task task)
        {
            try
            {
                await task;

                Assert.Fail("Action did not throw ApiException.");
            }
            catch (ApiException exception)
            {
                Assert.AreEqual(expectedHttpStatusCode, exception.HttpStatusCode);
            }
        }

        private static AuthenticationHandle GetAuthenticationHandle()
        {
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "apikey.txt");
            var apiKey = File.ReadAllText(configFilePath).Trim();

            return new AuthenticationHandle(apiKey, "test");
        }
    }
}
