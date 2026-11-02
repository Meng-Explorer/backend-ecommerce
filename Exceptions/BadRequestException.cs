namespace WepApi.Exceptions;

public class BadRequestException : System.Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, System.Exception innerException) : base(message, innerException)
    {
    }

    public string? Errors { get; set; }
}
