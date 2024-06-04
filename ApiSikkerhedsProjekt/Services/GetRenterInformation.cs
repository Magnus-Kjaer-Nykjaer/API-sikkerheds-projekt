using ApiSikkerhedsProjekt.Models;
using ApiSikkerhedsProjekt.Security;
using System.Data.SQLite;
using RepoDb;

namespace ApiSikkerhedsProjekt.Services
{
  public class GetRenterInformation(AccesController accesController, ILogger<GetRenterInformation> logger) : IGetRenterInformation
  {
    public async Task<RenterModel> GetRenter(int renterId, string apiKey)
    {
      try
      {
        if (!await accesController.CheckAccessToRenter(apiKey, renterId))
          return new RenterModel
          {
            RenterId = 0,
            ApartmentId = 0,
            ApartmentComplexId = 0,
            Name = "",
            Address = ""
          };

        var query = $"SELECT * FROM Renter where RenterId = @{nameof(renterId)}";

        await using SQLiteConnection conn = new SQLiteConnection("Data Source=mydatabase.db;");

        conn.Open();
        var result = await conn.ExecuteQueryAsync<RenterModel>(query, new
        {
          renterId
        });
        conn.Close();

        var renterModels = result.ToList();
        if (renterModels.Any())
          return renterModels.First();

        return new RenterModel
        {
          RenterId = 0,
          ApartmentId = 0,
          ApartmentComplexId = 0,
          Name = "",
          Address = ""
        };
      }
      catch (Exception e)
      {
        logger.LogCritical(e, "GetRenter has run into a problem, while getting the renter");
        throw;
      }
    }
  }
}
