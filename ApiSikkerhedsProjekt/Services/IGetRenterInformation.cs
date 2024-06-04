using ApiSikkerhedsProjekt.Models;

namespace ApiSikkerhedsProjekt.Services;

public interface IGetRenterInformation
{
  Task<RenterModel> GetRenter(int renterId, string apiKey);
}