using System.Linq;

namespace Nancy.CustomErrors
{
    public static class ResponseFormatterExtensions
    {
        public static Response AsError(this IResponseFormatter formatter, string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var serializer = formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/json"));

            return new ErrorResponse(new Error {Message = message}, serializer).WithStatusCode(statusCode);
        }
    }
}