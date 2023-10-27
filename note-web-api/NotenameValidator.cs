
public static class Validators
{
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
}