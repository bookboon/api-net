using System;
using System.Net;

namespace BookboonSDK
{
    /// <summary>
    /// Represents an error returned by the Bookboon API.
    /// </summary>
    public class ApiException : Exception
    {
        private readonly string _errorCode;
        private readonly HttpStatusCode _httpStatusCode;

        internal ApiException(string message, string errorCode, HttpStatusCode httpStatusCode, Exception innerException)
            : base(message, innerException)
        {
            _errorCode = errorCode;
            _httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Gets the system name of the exception from the Bookboon API.
        /// </summary>
        public string ErrorCode
        {
            get { return _errorCode; }
        }

        /// <summary>
        /// Gets the HTTP status code returned by the Bookboon API.
        /// </summary>
        public HttpStatusCode HttpStatusCode
        {
            get { return _httpStatusCode; }
        }
    }
}
