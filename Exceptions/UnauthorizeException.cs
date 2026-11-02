namespace WepApi.Exceptions;

public class UnauthorizeException : System.Exception
{
    public UnauthorizeException(string message = "You are not authorized to access this resource.") : base(message)
    {
    }

    public UnauthorizeException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
