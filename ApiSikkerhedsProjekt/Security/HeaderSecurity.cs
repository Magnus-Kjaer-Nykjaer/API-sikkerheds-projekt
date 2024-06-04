namespace ApiSikkerhedsProjekt.Security
{
  public class HeaderSecurity
  {
    private readonly ILogger<HeaderSecurity> _logger;

    public HeaderSecurity(ILogger<HeaderSecurity> logger)
    {
      _logger = logger;
    }

    public HttpRequest HeaderVerification(HttpRequest httpRequest)
    {
      try
      {
        httpRequest.Headers.XXSSProtection = "0";
        httpRequest.Headers.XContentTypeOptions = "nosniff";
        httpRequest.Headers.XFrameOptions = "deny";
        httpRequest.Headers.CacheControl = "no-store";
        httpRequest.Headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'; sandbox";

        return httpRequest;
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "HeaderVerification failed in validating the Headers");
        throw;
      }
    }

    public HttpResponse HeaderSanitization(HttpResponse httpResponse)
    {
      try
      {
        httpResponse.Headers.ContentType = "application/json";

        httpResponse.Headers.XXSSProtection = "0";
        httpResponse.Headers.XContentTypeOptions = "nosniff";
        httpResponse.Headers.XFrameOptions = "deny";
        httpResponse.Headers.CacheControl = "no-store";
        httpResponse.Headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'; sandbox";

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
