using System;
using System.IO;
using System.Linq;
using System.Net;
using Nancy.ErrorHandling;
using Nancy.Responses;
using Nancy.Responses.Negotiation;
using Nancy.ViewEngines;

namespace Nancy.CustomErrors
{
    public class ErrorStatusCodeHandler : DefaultViewRenderer, IStatusCodeHandler
    {
        private readonly ISerializer _serializer;

        public ErrorStatusCodeHandler(IViewFactory viewFactory)
            : this(viewFactory, new DefaultJsonSerializer())
        {
        }

        public ErrorStatusCodeHandler(IViewFactory viewFactory, ISerializer serializer)
            : base(viewFactory)
        {
            _serializer = serializer;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                   || statusCode == HttpStatusCode.InternalServerError
                   || statusCode == HttpStatusCode.Forbidden
                   || statusCode == HttpStatusCode.Unauthorized;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (!ShouldRenderFriendlyErrorPage(context))
            {
                if (context.Response is NotFoundResponse)
                {
                    // Normally we return 404's ourselves so we have an ErrorResponse. 
                    // But if no route is matched, Nancy will set a NotFound response itself. 
                    // When this happens we still want to return our nice JSON response.
                    context.Response = new ErrorResponse(new Error
                    {
                        Message = CustomErrors.Configuration.NotFoundSummary
                    }, _serializer).WithStatusCode(statusCode);
                } 
                else if (!(context.Response is ErrorResponse))
                {
                    switch (statusCode)
                    {
                        case HttpStatusCode.Forbidden :
                        case HttpStatusCode.Unauthorized :
                            context.Response = new ErrorResponse(new Error
                            {
                                Message = CustomErrors.Configuration.UnauthorizedSummary
                            }).WithStatusCode(statusCode);
                            break;
                        case HttpStatusCode.NotFound :
                            context.Response = new ErrorResponse(new Error
                            {
                                Message = CustomErrors.Configuration.NotFoundSummary
                            }).WithStatusCode(statusCode);
                            break;
                    }
                }

                // Pass the existing response through
                return;
            }
            
            var error = context.Response as ErrorResponse;

            var model = new ErrorViewModel
            {
                Details = error == null ? "" : error.FullException
            };
        
            switch (statusCode)
            {
                case HttpStatusCode.Forbidden:
                    model.Title = CustomErrors.Configuration.ForbiddenTitle;
                    model.Summary = CustomErrors.Configuration.ForbiddenSummary;

                    break;

                case HttpStatusCode.Unauthorized:
                    model.Title = CustomErrors.Configuration.UnauthorizedTitle;
                    model.Summary = error == null ? CustomErrors.Configuration.UnauthorizedSummary : error.ErrorMessage;
                                        
                    break;

                case HttpStatusCode.NotFound:
                    model.Title = CustomErrors.Configuration.NotFoundTitle;
                    model.Summary = CustomErrors.Configuration.NotFoundSummary;
                    
                    break;
                case HttpStatusCode.InternalServerError:
                    model.Title = CustomErrors.Configuration.ErrorTitle;
                    model.Summary = error == null ? CustomErrors.Configuration.ErrorSummary : error.ErrorMessage;
                    
                    break;
            }

            try
            {
                context.Response =
                    RenderView(context, CustomErrors.Configuration.ErrorViews[statusCode], model)
                        .WithStatusCode(statusCode);
            }
            catch(Exception e)
            {
                context.Response = new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Contents = stream =>
                    {
                        var writer = new StreamWriter(stream);
                        writer.AutoFlush = true;
                        writer.Write(string.Format("Could not locate your error view! Details: {0}", e.Message));
                    }
                };
            }
        }

        private static bool ShouldRenderFriendlyErrorPage(NancyContext context)
        {
            var ranges =
                context.Request.Headers.Accept.OrderByDescending(o => o.Item2)
                    .Select(o => MediaRange.FromString(o.Item1))
                    .ToList();

            foreach (var range in ranges)
            {
                if (range.Matches("application/json"))
                {
                    return false;
                }

                if (range.Matches("text/json"))
                {
                    return false;
                }

                if (range.Matches("text/html"))
                {
                    return true;
                }
            }

            return true;
        }
    }
}