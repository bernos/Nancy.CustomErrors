using System;
using System.Collections.Generic;

namespace Nancy.CustomErrors
{
    public class CustomErrorsConfiguration
    {
        /// <summary>
        /// Converts a thrown exception to the appropriate ErrorResponse. Override this method if you need
        /// to handle custom exception types, or implement your own error handling logic. The default 
        /// implementation converts all thrown exceptions to a regular ErrorResponse with an HttpStatusCode
        /// of 500
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public virtual ErrorResponse HandleError(NancyContext context, Exception ex, ISerializer serializer)
        {
            var error = new Error
            {
                FullException = ex.ToString(),
                Message = ex.Message
            };

            return new ErrorResponse(error, serializer).WithStatusCode(HttpStatusCode.InternalServerError) as ErrorResponse;
        }

        /// <summary>
        /// Maps different HttpStatusCodes to the appropriate views.
        /// </summary>
        public IDictionary<HttpStatusCode, string> ErrorViews = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.NotFound,              "Error" },
                { HttpStatusCode.InternalServerError,   "Error" },
                { HttpStatusCode.Forbidden,             "Error" }
            };
    }
}
