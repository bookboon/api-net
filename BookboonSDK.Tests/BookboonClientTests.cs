using System;
using System.IO;
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

            AssertThrowsApiException("NotFound", task);
        }

        [Test]
        public async void HttpsRequiredException()
        {
            var client = new BookboonClient();

            var task = client.Get("/recommendations");

            AssertThrowsApiException("HttpsRequired", task);
        }

        [Test]
        public async void ApiKeyInvalidException()
        {
            var client = new BookboonClient();
            var handle = new AuthenticationHandle("badapikey", "dummyhandle");

            var task = client.Get("/recommendations", handle);

            AssertThrowsApiException("ApiKeyInvalid", task);
        }

        [Test]
        public async void BadDataException()
        {
            var client = new BookboonClient();
            var handle = GetAuthenticationHandle();

            var task = client.Post("/profile", handle, new { email = "invalid", newsletter = false });

            AssertThrowsApiException("UnacceptableParameterValue", task);
        }

        public static async void AssertThrowsApiException(string expectedErrorCode, Task task)
        {
            try
            {
                await task;

                Assert.Fail("Action did not throw ApiException.");
            }
            catch (ApiException exception)
            {
                Assert.AreEqual(expectedErrorCode, exception.ErrorCode);
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
