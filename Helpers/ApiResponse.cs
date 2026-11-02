using Microsoft.AspNetCore.Mvc;

namespace WepApi.Helpers;


public static class ApiResponse
{
    public static IActionResult Success(object data, string message = "Success")
    {
        return new OkObjectResult(new
        {
            success = true,
            statusCode = 200,
            message = message,
            data = data
        });
    }

    public static object Error(string message, int statusCode = StatusCodes.Status400BadRequest)
    {
        return new
        {
            success = false,
            statusCode = statusCode,
            message = message,
            data = new { }
        };
    }
}
