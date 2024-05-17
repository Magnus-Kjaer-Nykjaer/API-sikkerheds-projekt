using System.Data.SQLite;

namespace ApiSikkerhedsProjekt.Security
{
  public class DatabaseHelperForSecurity
  {
    private readonly ILogger<DatabaseHelperForSecurity> _logger;

    private readonly string _connectionString = "Data Source=mydatabase.db;";

    public DatabaseHelperForSecurity(ILogger<DatabaseHelperForSecurity> logger)
    {
      _logger = logger;
    }

    public async Task<bool> ValidateKeySecret(string key, string secret)
    {
      if (!CheckIfTableExistsIfNotCreate()) return false;

      var query = @$"SELECT * FROM Security 
                      WHERE API-KEY = {key} 
                      AND API-SECRET = {secret}";

      using var conn = new SQLiteConnection(_connectionString);
      conn.Open();
      using var command = new SQLiteCommand(query, conn);

      command.ExecuteNonQuery();
      conn.Close();
      return true;
    }

    private bool CheckIfTableExistsIfNotCreate()
    {
      try
      {
        var query = "CREATE TABLE Security (id INTEGER PRIMARY KEY, API-KEY TEXT, API-SECRET uniqueidentifier)";

        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        using var command = new SQLiteCommand(query, conn);

        command.ExecuteNonQuery();
        conn.Close();

        return true;
      }
      catch (Exception e)
      {
        _logger.LogError(e, "CheckIfTableExistsIfNotCreate has run into a problem");
        return false;
      }
    }
  }
}
