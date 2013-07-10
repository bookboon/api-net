using System;
using System.Text;

namespace BookboonSDK
{
    /// <summary>
    /// Represents an authorization handle for the Bookboon API, consisting of an application
    /// API key and a uniquely identifying handle for the current end user.
    /// </summary>
    public class AuthenticationHandle
    {
        private readonly string _apiKey;
        private readonly string _handle;

        /// <summary>
        /// Instantiates a new <see cref="AuthenticationHandle"/> with a specific API key and user handle.
        /// </summary>
        /// <param name="apiKey">The secret Bookboon API key provided for your application.</param>
        /// <param name="handle">A unique string (max 64 characters) identifying the end user.</param>
        public AuthenticationHandle(string apiKey, string handle)
        {
            _apiKey = apiKey;
            _handle = handle;
        }
        
        internal string GetAuthorizationHeader()
        {
            var authInfo = _handle + ":" + _apiKey;
            var authBytes = Encoding.Default.GetBytes(authInfo);
            return "Basic " + Convert.ToBase64String(authBytes);
        }
    }
}
