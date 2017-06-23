// ***********************************************************************
// Assembly         : Nancy.CustomErrors.Tests
// Author           : 
// Created          : 05-13-2017
//
// Last Modified By : 
// Last Modified On : 06-23-2017
// ***********************************************************************
// <copyright file="CustomErrorsFixture.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using Nancy.Bootstrapper;
using NSubstitute;
using System;
using Xunit;

namespace Nancy.CustomErrors.Tests
{
	/// <summary>
	/// Class CustomErrorsFixture.
	/// </summary>
	public class CustomErrorsFixture
	{
		/// <summary>
		/// Shoulds the throw with null pipelines passed to enable.
		/// </summary>
		[Fact]
		public void Should_throw_with_null_pipelines_passed_to_enable()
		{
			Assert.Throws<ArgumentNullException>(() => CustomErrors.Enable(null, new CustomErrorsConfiguration()));
		}

		/// <summary>
		/// Shoulds the throw with null configuration passed to enable.
		/// </summary>
		[Fact]
		public void Should_throw_with_null_configuration_passed_to_enable()
		{
			Assert.Throws<ArgumentNullException>(() => CustomErrors.Enable(Substitute.For<IPipelines>(), null));
		}

		/// <summary>
		/// Shoulds the add error hook when enabled.
		/// </summary>
		[Fact]
		public void Should_add_error_hook_when_enabled()
		{
			var pipelines = Substitute.For<IPipelines>();
			pipelines.OnError.Returns(Substitute.For<ErrorPipeline>());

			try
			{
				var cfg = Substitute.For<CustomErrorsConfiguration>();

				CustomErrors.Enable(pipelines, cfg);

				pipelines.OnError.Received(1).AddItemToEndOfPipeline(Arg.Any<Func<NancyContext, Exception, Response>>());
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
