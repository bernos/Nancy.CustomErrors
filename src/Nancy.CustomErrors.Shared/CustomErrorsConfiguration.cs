// ***********************************************************************
// Assembly         : Nancy.CustomErrors
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-13-2017
// ***********************************************************************
// <copyright file="CustomErrorsConfiguration.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace Nancy.CustomErrors
{
	/// <summary>
	/// Class CustomErrorsConfiguration.
	/// </summary>
	public class CustomErrorsConfiguration
    {
		/// <summary>
		/// The not found title
		/// </summary>
		public string NotFoundTitle = "404 Not Found";
		/// <summary>
		/// The not found summary
		/// </summary>
		public string NotFoundSummary = "The requested resource could not be found.";
		/// <summary>
		/// The forbidden title
		/// </summary>
		public string ForbiddenTitle = "Forbidden";
		/// <summary>
		/// The forbidden summary
		/// </summary>
		public string ForbiddenSummary = "You do not have permission to do that.";
		/// <summary>
		/// The unauthorized title
		/// </summary>
		public string UnauthorizedTitle = "Unauthorized";
		/// <summary>
		/// The unauthorized summary
		/// </summary>
		public string UnauthorizedSummary = "You do not have permission to do that.";
		/// <summary>
		/// The error title
		/// </summary>
		public string ErrorTitle = "Error";
		/// <summary>
		/// The error summary
		/// </summary>
		public string ErrorSummary = "An unexpected error occurred.";

		/// <summary>
		/// The always return json
		/// </summary>
		public bool AlwaysReturnJson = false;

		/// <summary>
		/// If set to true, then we will emit full stack traces in our ErrorResponse
		/// </summary>
		public bool Debug = false;

		/// <summary>
		/// Converts a thrown exception to the appropriate ErrorResponse. Override this method if you need
		/// to handle custom exception types, or implement your own error handling logic. The default
		/// implementation converts all thrown exceptions to a regular ErrorResponse with an HttpStatusCode
		/// of 500
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="ex">The ex.</param>
		/// <param name="serializer">The serializer.</param>
		/// <returns>ErrorResponse.</returns>
		public virtual ErrorResponse HandleError(NancyContext context, Exception ex, ISerializer serializer)
        {
            var error = new Error
            {
                FullException = ex.ToString(),
                Message = ex.Message
            };

            return new ErrorResponse(error, serializer, context.Environment).WithStatusCode(HttpStatusCode.InternalServerError) as ErrorResponse;
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
