// ***********************************************************************
// Assembly         : Nancy.CustomErrors.Example
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-14-2017
// ***********************************************************************
// <copyright file="Program.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using Nancy.Hosting.Self;
using System;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Nancy.CustomErrors.Example
{
	/// <summary>
	/// Class Program.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		static void Main(string[] args)
		{
			var cfg = new HostConfiguration() {UrlReservations = new UrlReservations() {CreateAutomatically = true}};
			
			var uri = new Uri("http://localhost:1234");

			using (var host = new NancyHost(cfg, uri))
			{				
				host.Start();
				
				Console.WriteLine($"Launch a browser and navigate to {uri} (/test, /err, /json)");

				Console.WriteLine("Press any key to exit application");
				Console.ReadKey();
			}
		}
	}

	/// <summary>
	/// Class TestBootstrapper.
	/// </summary>
	/// <seealso cref="Nancy.DefaultNancyBootstrapper" />
	public class TestBootstrapper : DefaultNancyBootstrapper
	{
		/// <summary>
		/// Initialise the bootstrapper - can be used for adding pre/post hooks and
		/// any other initialisation tasks that aren't specifically container setup
		/// related
		/// </summary>
		/// <param name="container">Container instance for resolving types if required.</param>
		/// <param name="pipelines">Pipelines instance to be customized if required</param>
		protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup(container, pipelines);

			CustomErrors.Enable(pipelines, new ErrorConfiguration());			
		}

		/// <summary>
		/// Initialise the request - can be used for adding pre/post hooks and
		/// any other per-request initialisation tasks that aren't specifically container setup
		/// related
		/// </summary>
		/// <param name="container">Container</param>
		/// <param name="pipelines">Current pipelines</param>
		/// <param name="context">Current context</param>
		protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
		{
			Console.WriteLine($"\tRequested Url: {context.Request.Url}");

			base.RequestStartup(container, pipelines, context);
		}

	}

	/// <summary>
	/// Class TestModule.
	/// </summary>
	/// <seealso cref="Nancy.NancyModule" />
	public class TestModule : NancyModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TestModule"/> class.
		/// </summary>
		public TestModule()
			: base("/")
		{
			Get("/test", _ =>
			{
				var response = Response.AsText("test", "application/json");
				response.StatusCode = HttpStatusCode.InternalServerError;
				return response;
			});

			Get("/err", _ =>
			{
				throw new Exception("asdadsfdaf");
			});

			Get("/json", _ =>
			{
				var response = Response.AsError("json test", HttpStatusCode.InternalServerError);

				return response;
			});
		}
	}

	/// <summary>
	/// Class ErrorConfiguration.
	/// </summary>
	/// <seealso cref="Nancy.CustomErrors.CustomErrorsConfiguration" />
	public class ErrorConfiguration : CustomErrorsConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorConfiguration"/> class.
		/// </summary>
		public ErrorConfiguration()
		{
			// Map error status codes to custom view names
			ErrorViews[HttpStatusCode.NotFound] = "error";
			ErrorViews[HttpStatusCode.InternalServerError] = "error";
			ErrorViews[HttpStatusCode.Forbidden] = "error";
		}
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
		public override ErrorResponse HandleError(NancyContext context, Exception ex, ISerializer serializer)
		{
			var error = new Error
			{
				FullException = ex.ToString(),
				Message = ex.Message
			};

			return new ErrorResponse(error, serializer, context.Environment).WithStatusCode(HttpStatusCode.InternalServerError) as ErrorResponse;
		}
	}
}
