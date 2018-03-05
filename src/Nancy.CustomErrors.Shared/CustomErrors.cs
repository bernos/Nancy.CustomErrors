// ***********************************************************************
// Assembly         : Nancy.CustomErrors
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 06-23-2017
// ***********************************************************************
// <copyright file="CustomErrors.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Nancy.Bootstrapper;
using Nancy.Responses;

namespace Nancy.CustomErrors
{
	/// <summary>
	/// Class CustomErrors.
	/// </summary>
	public static class CustomErrors
	{
		/// <summary>
		/// The configuration
		/// </summary>
		private static CustomErrorsConfiguration _configuration;
		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public static CustomErrorsConfiguration Configuration
		{
			get { return _configuration ?? (_configuration = new CustomErrorsConfiguration()); }
		}

		/// <summary>
		/// Enables the specified pipelines.
		/// </summary>
		/// <param name="pipelines">The pipelines.</param>
		/// <param name="configuration">The configuration.</param>
		public static void Enable(IPipelines pipelines, CustomErrorsConfiguration configuration)
		{
			Enable(pipelines, configuration, new DefaultJsonSerializer(Nancy.Bootstrapper.NancyBootstrapperLocator.Bootstrapper.GetEnvironment()));
		}

		/// <summary>
		/// Enables the specified pipelines.
		/// </summary>
		/// <param name="pipelines">The pipelines.</param>
		/// <param name="configuration">The configuration.</param>
		/// <param name="serializer">The serializer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// pipelines
		/// or
		/// configuration
		/// </exception>
		public static void Enable(IPipelines pipelines, CustomErrorsConfiguration configuration, ISerializer serializer)
		{
			if (pipelines == null)
			{
				throw new ArgumentNullException(nameof(pipelines));
			}

			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			_configuration = configuration;
			
			pipelines.OnError.AddItemToEndOfPipeline(GetErrorHandler(configuration, serializer));
		}

		/// <summary>
		/// Gets the error handler.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="serializer">The serializer.</param>
		/// <returns>Func&lt;NancyContext, Exception, Response&gt;.</returns>
		private static Func<NancyContext, Exception, Response> GetErrorHandler(CustomErrorsConfiguration configuration, ISerializer serializer)
		{
			return (context, ex) => configuration.HandleError(context, ex, serializer);
		}
	}
}