using ApiSikkerhedsProjekt.Models;
using ApiSikkerhedsProjekt.Security;
using RepoDb;
using System.Data.SQLite;
using FluentResults;

namespace ApiSikkerhedsProjekt.Services
{
  public class GetRenterInformation(AccesController accesController, ILogger<GetRenterInformation> logger) : IGetRenterInformation
  {
    public async Task<Result<RenterModel>> GetRenter(int renterId, string apiKey)
    {
      try
      {
        if (!await accesController.CheckAccessToRenter(apiKey, renterId))
        {
          logger.LogWarning("API-Key {apikey} does not have access to the renter with the id {renterId}", apiKey, renterId );
          return Result.Fail("You do not have access to this renters information");
        }

        string query = @$"SELECT * FROM Renter where RenterId = @{nameof(renterId)}";

        await using SQLiteConnection conn = new SQLiteConnection("Data Source=mydatabase.db;");

        conn.Open();
        IEnumerable<RenterModel> result = await conn.ExecuteQueryAsync<RenterModel>(query, new
        {
          renterId
        });
        conn.Close();

        List<RenterModel> renterModels = result.ToList();
        if (renterModels.Any())
          return renterModels.First();

        return Result.Fail($"No renter found with the given RenterId");
      }
      catch (Exception e)
      {
        logger.LogCritical(e, "GetRenter has run into a problem, while getting the renter");
        throw;
      }
    }
  }
}
