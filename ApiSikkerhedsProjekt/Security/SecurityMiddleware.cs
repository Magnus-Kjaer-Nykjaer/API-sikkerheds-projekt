namespace ApiSikkerhedsProjekt.Security
{
  public class SecurityMiddleware : IMiddleware
  {
    private readonly ILogger<SecurityMiddleware> _logger;
    private static readonly Array _separator = new[] { '/' };
    private readonly DatabaseHelperForSecurity _apiSecurityHelper;
    private readonly HeaderSecurity _headerSecurity;

    public SecurityMiddleware(ILogger<SecurityMiddleware> logger, DatabaseHelperForSecurity apiSecurityHelper, HeaderSecurity headerSecurity)
    {
      _logger = logger;
      _apiSecurityHelper = apiSecurityHelper;
      _headerSecurity = headerSecurity;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      string controller = context.Request.Path.Value?
        .ToLower()
        .Split(_separator as char[], StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault() ?? string.Empty;

      if (!context.Request.IsHttps)
      {
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
      }

      var headerSanitization = _headerSecurity.HeaderSanitization(context.Response);
      foreach (var headerPair in headerSanitization.Headers) 
        context.Response.Headers.TryAdd(headerPair.Key, headerPair.Value);

      if (!controller.StartsWith("swagger"))
      {
        if (context.Request.Headers.ContentType != "application/json")
        {
          context.Response.Clear();
          context.Response.StatusCode = StatusCodes.Status400BadRequest;
          return;
        }
        if (!await ValidateCredentials(context.Request))
        {
          context.Response.Clear();
          context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
          await context.Response.WriteAsync("Unauthorized");
          return;
        }
      }
      await next(context);
    }

    private async Task<bool> ValidateCredentials(HttpRequest request)
    {
      try
      {
        string? key = string.Empty;
        string? secret = string.Empty;
        if (request.Headers.TryGetValue("api-key", out Microsoft.Extensions.Primitives.StringValues keys))
        {
          key = keys.FirstOrDefault();
        }

        if (request.Headers.TryGetValue("api-secret", out Microsoft.Extensions.Primitives.StringValues secrets))
        {
          secret = secrets.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
        {
          _logger.LogError("Security Fejl: key: {clientKey}, secret:{clientSecret}",
            key ?? "NoKeyFound", secret ?? "NoSecretFound");
          return false;
        }
        bool test = await _apiSecurityHelper.ValidateKeySecret(key, secret);
        return test;
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "ValidateCredentials failed in validating the credentials");
      }

      return false;
    }
  }
}
