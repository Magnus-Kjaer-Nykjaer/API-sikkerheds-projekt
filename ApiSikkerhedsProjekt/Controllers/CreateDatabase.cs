using Microsoft.AspNetCore.Mvc;
using RepoDb;
using System.Data.SQLite;

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
      string connectionString = "Data Source=mydatabase.db;";
      CreateTables(connectionString);
      InsertData(connectionString);

    }

    private void InsertData(string connectionString)
    {
      try
      {
        Guid guid = new Guid();
        string query = @$"INSERT INTO Security (APIKEY, APISECRET)
                      VALUES ('Test', '@{nameof(guid)}')";

        using SQLiteConnection conn = new SQLiteConnection(connectionString);
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
        string query = "CREATE TABLE IF NOT EXISTS Security (id INTEGER PRIMARY KEY, APIKEY TEXT, APISECRET uniqueidentifier)";

        using SQLiteConnection conn = new SQLiteConnection(connectionString);
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
