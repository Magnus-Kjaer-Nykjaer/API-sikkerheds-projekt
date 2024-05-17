using System.Data.SQLite;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet(Name = "CreateDatabase")]
    public void Get()
    {
      var connectionString = "Data Source=mydatabase.db;";

      var query = "CREATE TABLE person (id INTEGER PRIMARY KEY, name TEXT)";

      using var conn = new SQLiteConnection(connectionString);
      conn.Open();
      using var command = new SQLiteCommand(query, conn);

      command.ExecuteNonQuery();
      conn.Close();
    }

    private bool InsertData(string connectionString)
    {
      try
      {
        var guid = new Guid();
        var query = @$"INSERT INTO Security (API-KEY, API-SECRET)
                      VALUES ('Test', '{guid}')";

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        using var command = new SQLiteCommand(query, conn);

        command.ExecuteNonQuery();
        conn.Close();
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return false;
      }
    }
  }
}
