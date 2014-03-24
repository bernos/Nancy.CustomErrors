using System;
using Nancy.Bootstrapper;
using Nancy.Responses;

namespace Nancy.CustomErrors
{
    public class CustomErrors
    {
        private static CustomErrorsConfiguration _configuration;
        public static CustomErrorsConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new CustomErrorsConfiguration();
                }
                return _configuration;
            }
        }

        public static CustomErrors Enable(IPipelines pipelines)
        {
            return Enable(pipelines, new CustomErrorsConfiguration());
        }

        public static CustomErrors Enable(IPipelines pipelines, ISerializer serializer)
        {
            return Enable(pipelines, new CustomErrorsConfiguration(), serializer);
        }

        public static CustomErrors Enable(IPipelines pipelines, CustomErrorsConfiguration configuration)
        {
            return Enable(pipelines, configuration, new DefaultJsonSerializer());
        }

        public static CustomErrors Enable(IPipelines pipelines, CustomErrorsConfiguration configuration, ISerializer serializer)
        {
            _configuration = configuration;
            var customErrors = new CustomErrors(serializer);

            pipelines.OnError.AddItemToEndOfPipeline(customErrors.HandleError);

            return customErrors;
        }

        private CustomErrors(ISerializer serializer)
        {
            _serializer = serializer;
        }

        private readonly ISerializer _serializer;

        private Response HandleError(NancyContext context, Exception e)
        {
            return Configuration.HandleError(context, e, _serializer);
        }
    }
}