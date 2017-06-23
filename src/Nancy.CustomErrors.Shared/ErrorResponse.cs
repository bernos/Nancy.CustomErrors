// ***********************************************************************
// Assembly         : Nancy.CustomErrors
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-13-2017
// ***********************************************************************
// <copyright file="ErrorResponse.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Nancy.Configuration;
using Nancy.Responses;

namespace Nancy.CustomErrors
{
	/// <summary>
	/// Class ErrorResponse.
	/// </summary>
	public class ErrorResponse : JsonResponse
	{
		/// <summary>
		/// The error
		/// </summary>
		private readonly Error _error;
		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage { get { return _error.Message; } }
		/// <summary>
		/// Gets the full exception.
		/// </summary>
		/// <value>The full exception.</value>
		public string FullException { get { return _error.FullException; } }
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorResponse"/> class.
		/// </summary>
		/// <param name="error">The error.</param>
		/// <param name="serializer">The serializer.</param>
		/// <param name="environment">The environment.</param>
		public ErrorResponse(Error error, ISerializer serializer, INancyEnvironment environment) : base(error, serializer, environment)
		{
			if (!CustomErrors.Configuration.Debug)
			{
				error.FullException = null;
			}

			_error = error;
		}
	}
}
