namespace Application.Core
{
    public class AppException
    {
        //constructor
        //details have default value of null
        public AppException(int statusCode, string message, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        //properties
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}