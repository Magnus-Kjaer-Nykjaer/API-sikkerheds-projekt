using ApiSikkerhedsProjekt.Models;
using ApiSikkerhedsProjekt.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiSikkerhedsProjekt.Controllers
{
  [Route("[controller]")]
  public class GetRenterInformationController(IGetRenterInformation getRenterInformation) : ControllerBase
  {
    [HttpGet(Name = "GetRenterInformation")]
    public async Task<RenterModel> Get(int renterId)
    {
      var renter = new RenterModel
      {
        RenterId = 0,
        ApartmentId = 0,
        ApartmentComplexId = 0,
        Name = "",
        Address = ""
      };
      if (HttpContext.Request.Headers.TryGetValue("api-key", out Microsoft.Extensions.Primitives.StringValues keys))
      {
        renter = await getRenterInformation.GetRenter(renterId, keys.First() ?? string.Empty);
      }

      return renter;
    }
  }
}
