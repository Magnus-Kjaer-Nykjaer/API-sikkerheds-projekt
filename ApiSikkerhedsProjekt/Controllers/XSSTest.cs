using Microsoft.AspNetCore.Mvc;

namespace ApiSikkerhedsProjekt.Controllers
{
  [Route("[controller]")]
  public class XSSTest(ILogger<TestLogging> logger) : ControllerBase
  {
    [HttpPost(Name = "XSSTest")]
    public string Post([FromBody] string request)
    {
      logger.LogInformation("Logging works");
      return request;
    }
  }
}
