
using System.Text;

public static class CustomMiddleware
{
    private static string PASSWORD_HEADER = "Note-Taking-Password";
    private static string PASSWORD = "qwerty123";

    public static async ValueTask<object?> ValidateNotename(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
    {
        if (invocationContext.Arguments.Count == 0) return await next(invocationContext);

        var notename = invocationContext.GetArgument<string>(0);

        if (notename == null) return await next(invocationContext);
        if (notename.Contains('.') || notename.Contains('/') || notename.Contains('\\'))
        {
            return Results.BadRequest($"Note name may not contain any '.', '/' or '\\'.");
        }
        return await next(invocationContext);
    }

    public static Task CheckPassword(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers;
        if (!headers.ContainsKey(PASSWORD_HEADER)
            || !headers[PASSWORD_HEADER].Equals(PASSWORD)
        ) {
            var bodyBytes = Encoding.UTF8.GetBytes($"No '{PASSWORD_HEADER}' found or wrong password");
            context.Response.StatusCode = 403;
            context.Response.Body.WriteAsync(bodyBytes);
            return Task.CompletedTask;
        }

        return next(context);
    }
}