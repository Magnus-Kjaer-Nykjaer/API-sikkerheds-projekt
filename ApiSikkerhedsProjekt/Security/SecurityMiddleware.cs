using Microsoft.AspNetCore.Http;

namespace ApiSikkerhedsProjekt.Security
{
  public class SecurityMiddleware : IMiddleware
  {
    private readonly ILogger<SecurityMiddleware> _logger;
    private static readonly Array _separator = new[] { '/' };
    private readonly DatabaseHelperForSecurity _apiSecurityHelper;

    public SecurityMiddleware(ILogger<SecurityMiddleware> logger, DatabaseHelperForSecurity apiSecurityHelper)
    {
      _logger = logger;
      _apiSecurityHelper = apiSecurityHelper;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      var controller = context.Request.Path.Value?
        .ToLower()
        .Split(_separator as char[], StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault() ?? string.Empty;

      if (!context.Request.IsHttps)
      {
        context.Abort();
        return;
      }

      //var headerVerification = await _headerSecurity.HeaderVerification(context.Request);
      //context.Request = headerVerification;

      if (!controller.StartsWith("swagger"))
      {
        if (context.Request.Headers.ContentType != "application/json")
        {
          context.Abort();
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
        var key = string.Empty;
        var secret = string.Empty;
        if (request.Headers.TryGetValue("api-key", out var keys))
        {
          key = keys.FirstOrDefault();
        }

        if (request.Headers.TryGetValue("api-secret", out var secrets))
        {
          secret = secrets.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
        {
          _logger.LogError("Security Fejl: key: {clientKey}, secret:{clientSecret}",
            key ?? "NoKeyFound", secret ?? "NoSecretFound");
          return false;
        }
        var test = await _apiSecurityHelper.ValidateKeySecret(key, secret);
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
