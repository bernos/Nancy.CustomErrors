// ***********************************************************************
// Assembly         : Nancy.CustomErrors.Tests
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 05-13-2017
// ***********************************************************************
// <copyright file="ErrorStatusCodeHandlerFixture.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using Nancy.Configuration;
using Nancy.Responses;
using Nancy.Testing;
using System;
using System.Linq;
using Xunit;

namespace Nancy.CustomErrors.Tests
{
	/// <summary>
	/// Class ErrorStatusCodeHandlerFixture.
	/// </summary>
	public class ErrorStatusCodeHandlerFixture
	{
		/// <summary>
		/// The configuration
		/// </summary>
		private readonly CustomErrorsConfiguration configuration;
		/// <summary>
		/// The browser
		/// </summary>
		private readonly Browser browser;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorStatusCodeHandlerFixture"/> class.
		/// </summary>
		public ErrorStatusCodeHandlerFixture()
		{
			configuration = new CustomErrorsConfiguration();
			
			browser = new Browser(new ConfigurableBootstrapper(with =>
			{
				with.ApplicationStartup((container, pipelines) => CustomErrors.Enable(pipelines, configuration, new DefaultJsonSerializer(new DefaultNancyEnvironment())));
				with.Module<TestModule>();
				with.StatusCodeHandler<ErrorStatusCodeHandler>();
			}));
		}


		/// <summary>
		/// Shoulds the return custom error response for route not found.
		/// </summary>
		[Fact]
		public void Should_return_custom_error_response_for_route_not_found()
		{
			var response = browser.Get("/nuffin", with => with.Header("Accept", "application/json")).Result;

			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("The requested resource could not be found.", response.Body.DeserializeJson<Error>().Message);
		}

		/// <summary>
		/// Shoulds the return json for application json accept header.
		/// </summary>
		[Fact]
		public void Should_return_json_for_application_json_accept_header()
		{
			var response = browser.Get("/error", with => with.Header("Accept", "application/json")).Result;

			Assert.NotNull(response.Body.DeserializeJson<Error>());
		}

		/// <summary>
		/// Shoulds the return json for text json accept header.
		/// </summary>
		[Fact]
		public void Should_return_json_for_text_json_accept_header()
		{
			var response = browser.Get("/error", with => with.Header("Accept", "text/json")).Result;

			Assert.NotNull(response.Body.DeserializeJson<Error>());
		}

		/// <summary>
		/// Shoulds the return HTML for text HTML accept header.
		/// </summary>
		[Fact]
		public void Should_return_html_for_text_html_accept_header()
		{
			var response = browser.Get("/error", with => with.Header("Accept", "text/html")).Result;

			response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
		}

		/// <summary>
		/// Shoulds the return HTML no accept header.
		/// </summary>
		[Fact]
		public void Should_return_html_no_accept_header()
		{
			var response = browser.Get("/error").Result;

			response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
		}

		/// <summary>
		/// Shoulds the return custom error response for uncaught exception.
		/// </summary>
		[Fact]
		public void Should_return_custom_error_response_for_uncaught_exception()
		{
			var response = browser.Get("/error", with => with.Header("Accept", "application/json")).Result;

			Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
			Assert.Equal("ERROR MESSAGE HERE", response.Body.DeserializeJson<Error>().Message);
		}

		/// <summary>
		/// Shoulds the return custom error response for forbidden.
		/// </summary>
		[Fact]
		public void Should_return_custom_error_response_for_forbidden()
		{
			var response = browser.Get("forbidden", with => with.Header("Accept", "application/json")).Result;

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
			Assert.NotNull(response.Body.DeserializeJson<Error>());
		}

		/// <summary>
		/// Shoulds the return custom error response for unauthorised.
		/// </summary>
		[Fact]
		public void Should_return_custom_error_response_for_unauthorised()
		{
			var response = browser.Get("unauthorised", with => with.Header("Accept", "application/json")).Result;

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
			Assert.NotNull(response.Body.DeserializeJson<Error>());
		}

		/// <summary>
		/// Shoulds the render custom HTML for uncaught exception.
		/// </summary>
		[Fact]
		public void Should_render_custom_html_for_uncaught_exception()
		{
			var response = browser.Get("error").Result;

			response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
			response.Body["h1"].ShouldExistOnce().And.ShouldContain("ERROR MESSAGE HERE");
		}

		/// <summary>
		/// Shoulds the suppress full stack trace by default.
		/// </summary>
		[Fact]
		public void Should_suppress_full_stack_trace_by_default()
		{
			var response = browser.Get("/err", with => with.Header("Accept", "application/json")).Result;

			Assert.Null(response.Body.DeserializeJson<Error>().FullException);
		}

		/// <summary>
		/// Shoulds the expose full stack trace in debug mode.
		/// </summary>
		[Fact]
		public void Should_expose_full_stack_trace_in_debug_mode()
		{
			CustomErrors.Configuration.Debug = true;

			var response = browser.Get("/error", with => with.Header("Accept", "application/json")).Result;
			
			Assert.NotNull(response.Body.DeserializeJson<Error>().FullException);

			CustomErrors.Configuration.Debug = false;
		}

		/// <summary>
		/// Shoulds the retain headers already set.
		/// </summary>
		[Fact]
		public void Should_retain_headers_already_set()
		{
			var response = browser.Get("/headers", with => with.Header("Accept", "application/json")).Result;

			Assert.NotNull(response.Headers.Where(h => h.Key == "CustomHeader"));
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
		{
			Get("error", _ =>
			{
				throw new Exception("ERROR MESSAGE HERE");
			});

			Get("forbidden", _ => HttpStatusCode.Forbidden);
			Get("unauthorised", _ => HttpStatusCode.Unauthorized);
			Get("headers", 
				_ =>
					new Response().WithStatusCode(HttpStatusCode.InternalServerError)
						.WithHeader("CustomHeader", "CustomHeaderValue"));
		}
	}
}
