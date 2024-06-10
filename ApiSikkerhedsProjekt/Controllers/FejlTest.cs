using Microsoft.AspNetCore.Mvc;

namespace ApiSikkerhedsProjekt.Controllers
{
  [Route("[controller]")]
  public class FejlTest(ILogger<TestLogging> logger) : ControllerBase
  {
    [HttpGet(Name = "FejlTest")]
    public ActionResult<DateTime> Get(string request)
    {
      try
      {
        var result = Convert.ToDateTime(request);
        return Ok(result);
      }
      catch (Exception e)
      {
        logger.LogCritical(e, "FejlTest failed in converting to datetime");
        return BadRequest(e.Message);
      }
    }
  }
}
