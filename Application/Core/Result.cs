namespace Application.Core
{
    //Generic object type T, but will take Activity or null
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }

        //Will either an activity or null
        public static Result<T> Success(T value) => new Result<T> {IsSuccess = true, Value = value};
        //Deals with errors in Handler
        public static Result<T> Failure(string error) => new Result<T> {IsSuccess = false, Error = error};
    }
}