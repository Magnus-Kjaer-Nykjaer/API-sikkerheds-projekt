using RepoDb;
using System.Data.SQLite;

namespace ApiSikkerhedsProjekt.DatabaseCreation
{
  public class DatabaseHelperForAccessControl(ILogger<DatabaseHelperForAccessControl> logger)
  {
    private readonly string _connectionString = "Data Source=mydatabase.db;";

    public async Task<bool> ValidateAccessToRenter(string key, int renterId)
    {
      if (!await CheckIfTableExistsIfNotCreate()) return false;

      string query = @$"SELECT id FROM AccessControl 
                      WHERE APIKEY = @{nameof(key)} 
                      AND RenterId = @{nameof(renterId)} ";

      await using SQLiteConnection conn = new SQLiteConnection(_connectionString);
      conn.Open();
      IEnumerable<object> result = await conn.ExecuteQueryAsync<object>(query, new
      {
        key,
        renterId
      });
      conn.Close();
      return result.Any();
    }

    private async Task<bool> CheckIfTableExistsIfNotCreate()
    {
      try
      {
        string query = "CREATE TABLE IF NOT EXISTS AccessControl (id INTEGER PRIMARY KEY NOT NULL, APIKEY TEXT NOT NULL, RenterId INTEGER NOT NULL)";

        await using SQLiteConnection conn = new SQLiteConnection(_connectionString);
        conn.Open();
        int res = await conn.ExecuteNonQueryAsync(query);
        conn.Close();

        InsertIntoSecurity(_connectionString);

        return true;
      }
      catch (Exception e)
      {
        logger.LogError(e, "CheckIfTableExistsIfNotCreate has run into a problem");
        return false;
      }
    }

    private async void InsertIntoSecurity(string connectionString)
    {
      try
      {
        if (await CheckIfTableContainsValues(connectionString)) return;

        string guid = Guid.NewGuid().ToString();
        string query = @"INSERT INTO AccessControl (id, APIKEY, RenterId)
                      VALUES (1,'Test', 1)";

        await using SQLiteConnection conn = new SQLiteConnection(connectionString);
        conn.Open();
        await conn.ExecuteNonQueryAsync(query, new
        {
          guid
        });
        conn.Close();
      }
      catch (Exception e)
      {
        logger.LogCritical(e, "InsertData failed in inserting into Security");
        throw;
      }
    }

    private async Task<bool> CheckIfTableContainsValues(string connectionString)
    {
      await using SQLiteConnection conn = new SQLiteConnection(connectionString);
      conn.Open();
      IEnumerable<object> result = await conn.ExecuteQueryAsync<object>("SELECT * FROM AccessControl");
      conn.Close();
      if (result.Any())
      {
        return true;
      }
      return false;
    }
  }
}
