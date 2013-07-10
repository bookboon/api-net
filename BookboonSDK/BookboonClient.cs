using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookboonSDK
{
    /// <summary>
    /// Provides a simple, asynchronized interface to the Bookboon API.
    /// For more information about the API, please refer to
    /// https://github.com/bookboon/api
    /// </summary>
    public class BookboonClient
    {
        /// <summary>
        /// Issue an asynchronous GET request for a specific path in the Bookboon API.
        /// The request can optionally be authenticated and/or parameterized.
        /// </summary>
        /// <param name="path">The path to be requested from the API, eg /categories</param>
        /// <param name="handle">Optionally, an <see cref="AuthenticationHandle"/> identifying the user making the request.</param>
        /// <param name="parameters">Optionally, an object that contains the parameters for a route. The parameters are retrieved through
        /// reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
        /// <returns></returns>
        public async Task<dynamic> Get(string path, AuthenticationHandle handle = null, object parameters = null)
        {
            var requestUri = GetRequestUri(path, handle != null);

            if (parameters != null)
            {
                requestUri += '?' + FormatParameterString(parameters);
            }

            var request = GetWebRequest(requestUri, handle);

            return await PerformRequest(request);
        }

        /// <summary>
        /// Issue an asynchronous POST request for a specific path in the Bookboon API.
        /// The request can optionally be authenticated and/or parameterized.
        /// </summary>
        /// <param name="path">The path to be requested from the API, eg /categories</param>
        /// <param name="handle">Optionally, an <see cref="AuthenticationHandle"/> identifying the user making the request.</param>
        /// <param name="parameters">Optionally, an object that contains the parameters for a route. The parameters are retrieved through
        /// reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
        /// <returns></returns>
        public async Task<dynamic> Post(string path, AuthenticationHandle handle = null, object parameters = null)
        {
            var requestUri = GetRequestUri(path, handle != null);
            
            var request = GetWebRequest(requestUri, handle);
            request.Method = WebRequestMethods.Http.Post;

            if (parameters != null)
            {
                request.ContentType = "application/x-www-form-urlencoded";

                using (var stream = await Task.Factory.FromAsync(request.BeginGetRequestStream, x => request.EndGetRequestStream(x), null))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(FormatParameterString(parameters));
                }
            }

            return await PerformRequest(request);
        }

        private static string GetRequestUri(string path, bool authenticated)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return (authenticated ? "https" : "http") + "://api.bookboon.com/" + path.Trim('/');
        }

        private static string FormatParameterString(object parameters)
        {
            var tokens = parameters
                .GetType().GetProperties()
                .Select(x => Uri.EscapeDataString(x.Name) + '=' +
                    Uri.EscapeDataString(x.GetValue(parameters).ToString()));

            return string.Join("&", tokens);
        }

        private static HttpWebRequest GetWebRequest(string requestUri, AuthenticationHandle handle)
        {
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            
            if (handle != null)
            {
                request.Headers["Authorization"] = handle.GetAuthorizationHeader();
            }

            return request;
        }

        private static async Task<object> PerformRequest(HttpWebRequest request)
        {
            try
            {
                var response = await Task.Factory.FromAsync(request.BeginGetResponse, x => request.EndGetResponse(x), null);

                return ParseResponse(response);
            }
            catch (WebException exception)
            {
                if (exception.Response == null) throw;

                var jsonData = ParseResponse(exception.Response);
                var httpWebResponse = (HttpWebResponse) exception.Response;

                throw new ApiException((string) jsonData.message, (string) jsonData.error, httpWebResponse.StatusCode, exception);
            }
        }

        private static dynamic ParseResponse(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var textReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize(jsonReader);
            }
        }
    }
}
