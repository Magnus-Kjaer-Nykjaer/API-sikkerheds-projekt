using RepoDb;
using System.Data.SQLite;

namespace ApiSikkerhedsProjekt.DatabaseCreation
{
  public class DatabaseHelperForRenter
  {
    private readonly ILogger<DatabaseHelperForRenter> _logger;

    private readonly string _connectionString = "Data Source=mydatabase.db;";

    public DatabaseHelperForRenter(ILogger<DatabaseHelperForRenter> logger)
    {
      _logger = logger;
    }

    public async Task<bool> CheckIfTableExistsIfNotCreate()
    {
      try
      {
        string query = "CREATE TABLE IF NOT EXISTS Renter (RenterId INTEGER PRIMARY KEY NOT NULL, ApartmentId INTEGER NOT NULL, ApartmentComplexId INTEGER NOT NULL, Name TEXT NOT NULL, Address TEXT NOT NULL)";

        await using SQLiteConnection conn = new SQLiteConnection(_connectionString);
        conn.Open();
        int res = await conn.ExecuteNonQueryAsync(query);
        conn.Close();

        InsertIntoRenter(_connectionString);

        return true;
      }
      catch (Exception e)
      {
        _logger.LogError(e, "CheckIfTableExistsIfNotCreate has run into a problem");
        return false;
      }
    }

    private async void InsertIntoRenter(string connectionString)
    {
      try
      {
        if (await CheckIfTableContainsValues(connectionString)) return;
        int renterId;
        string name = "GDPRSEN ";
        string query = @$"INSERT INTO Renter (RenterId, ApartmentId, ApartmentComplexId, Name, Address)
                      VALUES (@{nameof(renterId)},1,1,@{nameof(name)},'Seebladsgade 1')";

        await using SQLiteConnection conn = new SQLiteConnection(connectionString);
        conn.Open();
        for (int id = 1; id < 15; id++)
        {
          renterId = id;
          name += id;
          await conn.ExecuteNonQueryAsync(query, new
          {
            renterId,
            name
          });

        }
        conn.Close();
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "InsertData failed in inserting into Renter");
        throw;
      }
    }

    private async Task<bool> CheckIfTableContainsValues(string connectionString)
    {
      await using SQLiteConnection conn = new SQLiteConnection(connectionString);
      conn.Open();
      IEnumerable<object> result = await conn.ExecuteQueryAsync<object>("SELECT * FROM Renter");
      conn.Close();
      if (result.Any())
      {
        return true;
      }
      return false;
    }
  }
}
