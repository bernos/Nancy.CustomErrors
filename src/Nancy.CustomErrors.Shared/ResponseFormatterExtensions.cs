// ***********************************************************************
// Assembly         : Nancy.CustomErrors
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-13-2017
// ***********************************************************************
// <copyright file="ResponseFormatterExtensions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using Nancy.Responses.Negotiation;

namespace Nancy.CustomErrors
{
	/// <summary>
	/// Class ResponseFormatterExtensions.
	/// </summary>
	public static class ResponseFormatterExtensions
	{
		/// <summary>
		/// Ases the error.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		/// <param name="message">The message.</param>
		/// <param name="statusCode">The status code.</param>
		/// <returns>Response.</returns>
		public static Response AsError(this IResponseFormatter formatter, string message,
			HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
		{
			var serializer = formatter.SerializerFactory.GetSerializer(new MediaRange("application/json"));

			return new ErrorResponse(new Error {Message = message}, serializer, formatter.Environment).WithStatusCode(statusCode);
		}
	}
}