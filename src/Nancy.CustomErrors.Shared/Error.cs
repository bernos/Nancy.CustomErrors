// ***********************************************************************
// Assembly         : Nancy.CustomErrors
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-13-2017
// ***********************************************************************
// <copyright file="Error.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Nancy.CustomErrors
{
	/// <summary>
	/// Class Error.
	/// </summary>
	public class Error
    {
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get; set; }
		/// <summary>
		/// Gets or sets the full exception.
		/// </summary>
		/// <value>The full exception.</value>
		public string FullException { get; set; }
    }
}