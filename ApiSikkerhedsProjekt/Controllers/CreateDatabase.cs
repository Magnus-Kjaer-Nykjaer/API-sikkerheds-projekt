using System.Data.SQLite;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using RepoDb;

namespace ApiSikkerhedsProjekt.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CreateDatabase : ControllerBase
  {

    private readonly ILogger<CreateDatabase> _logger;

    public CreateDatabase(ILogger<CreateDatabase> logger)
    {
      _logger = logger;
    }

    [HttpGet(Name = "CreateDatabaseAndPopulate")]
    public void Get()
    {
      var connectionString = "Data Source=mydatabase.db;";
      CreateTables(connectionString);
      InsertData(connectionString);

    }

    private void InsertData(string connectionString)
    {
      try
      {
        var guid = new Guid();
        var query = @$"INSERT INTO Security (APIKEY, APISECRET)
                      VALUES ('Test', '@{nameof(guid)}')";

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        conn.ExecuteNonQuery(query, new
        {
          guid
        });
        conn.Close();
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "InsertData failed in inserting into Security");
        throw;
      }
    }

    private void CreateTables(string connectionString)
    {
      try
      {
        var query = "CREATE TABLE IF NOT EXISTS Security (id INTEGER PRIMARY KEY, APIKEY TEXT, APISECRET uniqueidentifier)";

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        conn.ExecuteNonQuery(query);
        conn.Close();
      }
      catch (Exception e)
      {
        _logger.LogCritical(e, "CreateTables failed in creating Security");
        throw;
      }
    }
  }
}
