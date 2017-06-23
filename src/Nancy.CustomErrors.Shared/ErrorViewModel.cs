// ***********************************************************************
// Assembly         : Nancy.CustomErrors
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-13-2017
// ***********************************************************************
// <copyright file="ErrorViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Nancy.CustomErrors
{
	/// <summary>
	/// Class ErrorViewModel.
	/// </summary>
	public class ErrorViewModel
	{
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }
		/// <summary>
		/// Gets or sets the summary.
		/// </summary>
		/// <value>The summary.</value>
		public string Summary { get; set; }
		public string Message { get; set; }
		/// <summary>
		/// Gets or sets the details.
		/// </summary>
		/// <value>The details.</value>
		public string Details { get; set; }
	}
}
