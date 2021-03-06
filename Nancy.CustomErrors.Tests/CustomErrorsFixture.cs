﻿using System;
using System.Net;
using Nancy.Bootstrapper;
using Nancy.Testing;
using NSubstitute;
using Xunit;

namespace Nancy.CustomErrors.Tests
{
    public class CustomErrorsFixture
    {
        [Fact]
        public void Should_throw_with_null_pipelines_passed_to_enable()
        {
            Assert.Throws<ArgumentNullException>(() => CustomErrors.Enable(null, new CustomErrorsConfiguration()));
        }

        [Fact]
        public void Should_throw_with_null_configuration_passed_to_enable()
        {
            Assert.Throws<ArgumentNullException>(() => CustomErrors.Enable(Substitute.For<IPipelines>(), null));
        }

        [Fact]
        public void Should_add_error_hook_when_enabled()
        {
            var pipelines = Substitute.For<IPipelines>();
            pipelines.OnError.Returns(Substitute.For<ErrorPipeline>());

            CustomErrors.Enable(pipelines, Substitute.For<CustomErrorsConfiguration>());

            pipelines.OnError.Received(1).AddItemToEndOfPipeline(Arg.Any<Func<NancyContext, Exception, Response>>());
        }
    }
}
