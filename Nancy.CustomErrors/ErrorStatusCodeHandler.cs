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
            var clientWantsHtml = ShouldRenderFriendlyErrorPage(context);
            
            if (!clientWantsHtml)
            {
                if (context.Response is NotFoundResponse)
                {
                    // Normally we return 404's ourselves so we have an ErrorResponse. 
                    // But if no route is matched, Nancy will set a NotFound response itself. 
                    // When this happens we still want to return our nice JSON response.
                    context.Response = new ErrorResponse(new Error
                    {
                        Message = "The requested resource could not be found"
                    }, _serializer).WithStatusCode(statusCode);
                }

                // Pass the existing response through
                return;
            }
            
            var error = context.Response as ErrorResponse;

            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    var redirectUrl = CustomErrors.Configuration.GetAuthorizationUrl(context);

                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        context.Response = RenderView(context, CustomErrors.Configuration.ErrorViews[HttpStatusCode.Forbidden], new
                        {
                            Title = "Forbidden",
                            Summary = error == null ? "You do not have permission to do that." : error.ErrorMessage
                        }).WithStatusCode(statusCode);
                    }
                    else
                    {
                        context.Response = new RedirectResponse(redirectUrl);
                    }
                    
                    break;
                case HttpStatusCode.Forbidden:
                    context.Response = RenderView(context, CustomErrors.Configuration.ErrorViews[HttpStatusCode.Forbidden], new
                    {
                        Title = "Forbidden",
                        Summary = error == null ? "You do not have permission to do that." : error.ErrorMessage
                    }).WithStatusCode(statusCode);

                    break;
                case HttpStatusCode.NotFound:
                    context.Response = RenderView(context, CustomErrors.Configuration.ErrorViews[HttpStatusCode.NotFound], new
                    {
                        Title = "404 Not Found",
                        Summary = "Sorry, the resource you requested was not found."
                    }).WithStatusCode(statusCode); ;
                    
                    break;
                case HttpStatusCode.InternalServerError:
                    context.Response = RenderView(context, CustomErrors.Configuration.ErrorViews[HttpStatusCode.InternalServerError], new
                    {
                        Title = "Sorry, something went wrong",
                        Summary = error == null ? "An unexpected error occurred." : error.ErrorMessage,
                        Details = error == null ? null : error.FullException
                    }).WithStatusCode(statusCode); ;
                    break;
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