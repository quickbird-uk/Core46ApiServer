using Microsoft.AspNetCore.Mvc;

namespace Qb.Core46Api.Helpers
{
    /// <summary>Helpers for generating `IActionResult`s.</summary>
    public static class Res
    {
        /// <summary>Creates a `text/plain` response with default statuscode 200.</summary>
        /// <param name="content">Response content.</param>
        /// <param name="code">HTTP status code</param>
        /// <returns>Content result `text/plain` with specified status code.</returns>
        public static ContentResult PlainUtf8(string content, int code = 200)
        {
            return new ContentResult
            {
                Content = content,
                ContentType = "text/plain; charset=utf-8",
                StatusCode = 200
            };
        }

        /// <summary>Sends {"error": errorMessage} with application/json and status 400 explicitly set.</summary>
        /// <param name="errorMessage">The message to put in the json.</param>
        /// <param name="statusCode">Status code.</param>
        /// <returns>A Json ContentResult.</returns>
        public static ContentResult JsonErrorResult(string errorMessage, int statusCode)
        {
            var cr = new ContentResult
            {
                StatusCode = statusCode,
                Content = "{\"error\" : \"" + errorMessage + "\"}",
                ContentType = "application/json"
            };
            return cr;
        }
    }
}