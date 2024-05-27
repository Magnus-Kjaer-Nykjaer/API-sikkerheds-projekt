using Microsoft.AspNetCore.Mvc;
namespace ApiSikkerhedsProjekt.Controllers
{
  [Route("[controller]")]
  public class TestLogging(ILogger<TestLogging> logger) : ControllerBase
  {
    [HttpGet(Name = "TestLogging")]
    public string Get(string test)
    {
      logger.LogInformation("Logging works");
      return "Logging works";
    }
  }
}
