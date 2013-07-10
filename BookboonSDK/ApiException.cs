using System;
using System.Net;

namespace BookboonSDK
{
    /// <summary>
    /// Represents an error returned by the Bookboon API.
    /// </summary>
    public class ApiException : Exception
    {
        private readonly string _name;
        private readonly HttpStatusCode _httpStatusCode;

        internal ApiException(string message, string name, HttpStatusCode httpStatusCode, Exception innerException)
            : base(message, innerException)
        {
            _name = name;
            _httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Gets the system name of the exception from the Bookboon API.
        /// </summary>
        public string Name
        {
            get { return _name; }
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
