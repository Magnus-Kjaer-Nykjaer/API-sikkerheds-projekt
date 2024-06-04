using ApiSikkerhedsProjekt.Models;
using ApiSikkerhedsProjekt.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiSikkerhedsProjekt.Controllers
{
  [Route("[controller]")]
  public class GetRenterInformationController(IGetRenterInformation getRenterInformation) : ControllerBase
  {
    [HttpGet(Name = "GetRenterInformation")]
    public async Task<ActionResult<RenterModel>> Get(int renterId)
    {
      if (!HttpContext.Request.Headers.TryGetValue("api-key", out Microsoft.Extensions.Primitives.StringValues keys))
        return BadRequest("Missing api-key");
      var result = await getRenterInformation.GetRenter(renterId, keys.First() ?? string.Empty);
      if (result.IsFailed)
        return BadRequest(result.Errors.FirstOrDefault()?.Message);
      if (result.IsSuccess)
        return result.Value;

      return BadRequest("Something went wrong, try again");
    }
  }
}
