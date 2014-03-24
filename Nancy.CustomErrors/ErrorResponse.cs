using Nancy.Responses;

namespace Nancy.CustomErrors
{
    public class ErrorResponse : JsonResponse
    {
        private readonly Error _error;
        public string ErrorMessage { get { return _error.Message; } }
        public string FullException { get { return _error.FullException; } }
        public ErrorResponse(Error error) : this(error, new DefaultJsonSerializer()) { }
        public ErrorResponse(Error error, ISerializer serializer) : base(error, serializer)
        {
            _error = error;
        }
    }
}
