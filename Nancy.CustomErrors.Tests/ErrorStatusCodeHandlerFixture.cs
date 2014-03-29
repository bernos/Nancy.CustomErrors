﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Testing;
using Xunit;

namespace Nancy.CustomErrors.Tests
{
    public class ErrorStatusCodeHandlerFixture
    {
        private readonly ConfigurableBootstrapper bootstrapper;
        private readonly CustomErrorsConfiguration configuration;

        public ErrorStatusCodeHandlerFixture()
        {
            configuration = new CustomErrorsConfiguration();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.ApplicationStartup((container, pipelines) => CustomErrors.Enable(pipelines, configuration));
                with.Module<TestModule>();
                with.StatusCodeHandler<ErrorStatusCodeHandler>();
            });
        }


        [Fact]
        public void Should_return_custom_error_response_for_route_not_found()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("/nuffin", with => with.Header("Accept", "application/json"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("The requested resource could not be found", response.Body.DeserializeJson<Error>().Message);
        }

        [Fact]
        public void Should_return_json_for_application_json_accept_header()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("/error", with => with.Header("Accept", "application/json"));

            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }

        [Fact]
        public void Should_return_json_for_text_json_accept_header()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("/error", with => with.Header("Accept", "text/json"));

            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }
        
        [Fact]
        public void Should_return_html_for_text_html_accept_header()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("/error", with => with.Header("Accept", "text/html"));

            response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
        }

        [Fact]
        public void Should_return_html_no_accept_header()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("/error");

            response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
        }

        [Fact]
        public void Should_return_custom_error_response_for_uncaught_exception()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("error", with => with.Header("Accept", "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("ERROR MESSAGE HERE", response.Body.DeserializeJson<Error>().Message);
        }

        [Fact]
        public void Should_return_custom_error_response_for_forbidden()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("forbidden", with => with.Header("Accept", "application/json"));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }

        [Fact]
        public void Should_return_custom_error_response_for_unauthorised()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("unauthorised", with => with.Header("Accept", "application/json"));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }

        [Fact]
        public void Should_render_custom_html_for_uncaught_exception()
        {
            var browser = new Browser(bootstrapper);

            var response = browser.Get("error");

            response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
            response.Body["h1"].ShouldExistOnce().And.ShouldContain("ERROR MESSAGE HERE");
        }
    }


    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["error"] = _ =>
            {
                throw new Exception("ERROR MESSAGE HERE");
            };

            Get["forbidden"] = _ => HttpStatusCode.Forbidden;
            Get["unauthorised"] = _ => HttpStatusCode.Unauthorized;
        }
    }
}
