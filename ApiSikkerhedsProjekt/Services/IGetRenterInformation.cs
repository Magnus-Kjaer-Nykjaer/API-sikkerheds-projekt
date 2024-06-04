using ApiSikkerhedsProjekt.Models;
using FluentResults;

namespace ApiSikkerhedsProjekt.Services;

public interface IGetRenterInformation
{
  Task<Result<RenterModel>> GetRenter(int renterId, string apiKey);
}