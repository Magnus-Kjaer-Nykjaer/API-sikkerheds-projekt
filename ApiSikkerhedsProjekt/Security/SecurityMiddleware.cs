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
      if (!controller.StartsWith("swagger"))
      {
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
        //if (request.Headers.TryGetValue("api-key", out var headerValues))
        //{
        //  var headerValue = headerValues.FirstOrDefault();
        //  if (headerValue is null)
        //  {
        //    _logger.LogError("Forkert api-key {headerValue}", headerValue ?? "NoApiKeyFound");
        //    return false;
        //  }
        //  if (!headerValue.Equals("5C37881C-150F-42EC-941D-A35620E46036"))
        //  {
        //    _logger.LogError("Forkert api-key {headerValue}", headerValue ?? "NoApiKeyFound");
        //    return false;
        //  }
        //}
        //else
        //{
        //  _logger.LogError("Manglende API-KEY");
        //  return false;
        //}

        var clientKey = string.Empty;
        var clientSecret = string.Empty;
        if (request.Headers.TryGetValue("api-key", out var clientKeys))
        {
          clientKey = clientKeys.FirstOrDefault();
        }

        if (request.Headers.TryGetValue("api-secret", out var clientSecrets))
        {
          clientSecret = clientSecrets.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(clientKey) || string.IsNullOrEmpty(clientSecret))
        {
          _logger.LogError("Security Fejl: clientkey: {clientKey}, clientSecret:{clientSecret}",
            clientKey ?? "NoClientKeyFound", clientSecret ?? "NoClientSecretFound");
          return false;
        }
        return await _apiSecurityHelper.ValidateKeySecret(clientKey, clientSecret);
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "ValidateCredentials failed in validating the credentials");
      }

      return false;
    }

    private async Task<HttpRequest> HeaderVerification(HttpRequest httpRequest)
    {
      try
      {
        return httpRequest;
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "HeaderVerification failed in validating the Headers");
        throw;
      }
    }

    private async Task<HttpResponse> HeaderSanitization(HttpResponse httpResponse)
    {
      try
      {
        return httpResponse;
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "HeaderSanitization failed in sanitizing the Headers");
        throw;
      }
    }
  }
}
