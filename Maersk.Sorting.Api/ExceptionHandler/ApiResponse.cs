namespace Maersk.Sorting.Api.ExceptionHandler
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            string errorResponse = string.Empty;
            switch (statusCode)
            {
                case 400: errorResponse = "Bad request"; break;
                case 401: errorResponse = "UnAuthorized request"; break;
                case 404: errorResponse = "Resource not found"; break;
                case 500: errorResponse = "Internal server error"; break;
                default:
                    errorResponse = string.Empty;
                    break;
            };

            return errorResponse;
        }
    }
}
