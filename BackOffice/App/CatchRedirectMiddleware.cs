public class CatchRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public CatchRedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using (var newResponseBodyStream = new MemoryStream())
        {
            await _next(context);

            context.Response.Body = newResponseBodyStream;

            if (context.Response.StatusCode == StatusCodes.Status302Found &&
                !(  context.Request.Path.Equals("/api/Login/login") || 
                    context.Request.Path.Equals("/signin-google") || 
                    context.Request.Path.Equals("/api/login/redirect-to-frontend")))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            await newResponseBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}