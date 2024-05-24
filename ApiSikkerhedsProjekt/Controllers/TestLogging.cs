using Microsoft.AspNetCore.Mvc;
namespace ApiSikkerhedsProjekt.Controllers
{
  [Route("[controller]")]
  public class TestLogging(ILogger<TestLogging> logger) : ControllerBase
  {
    [HttpGet(Name = "TestLogging")]
    public void Get()
    {
      logger.LogInformation("Logging works");
    }
  }
}
