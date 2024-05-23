using System;
using System.Data.SQLite;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using RepoDb;

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
      if (!await CheckIfTableExistsIfNotCreate()) return false;

      var query = @$"SELECT * FROM Security 
                      WHERE APIKEY = @{nameof(key)} 
                      AND APISECRET = @{nameof(secret)} ";

      using var conn = new SQLiteConnection(_connectionString);
      conn.Open();
      var result = await conn.ExecuteQueryAsync<object>(query, new
      {
        key,
        secret
      });
      conn.Close();
      return result.Any();
    }

    private async Task<bool> CheckIfTableExistsIfNotCreate()
    {
      try
      {
        var query = "CREATE TABLE IF NOT EXISTS Security (id INTEGER PRIMARY KEY, APIKEY TEXT, APISECRET uniqueidentifier)";

        await using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var res = await conn.ExecuteNonQueryAsync(query);
        conn.Close();

        InsertData(_connectionString);

        return true;
      }
      catch (Exception e)
      {
        _logger.LogError(e, "CheckIfTableExistsIfNotCreate has run into a problem");
        return false;
      }
    }

    private async void InsertData(string connectionString)
    {
      try
      {
        if (await CheckIfTableContainsValues(connectionString)) return;

        var guid = new Guid();
        var query = @$"INSERT INTO Security (APIKEY, APISECRET)
                      VALUES ('Test', @{nameof(guid)})";

        await using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        await conn.ExecuteNonQueryAsync(query, new
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

    private async Task<bool> CheckIfTableContainsValues(string connectionString)
    {
      await using var conn = new SQLiteConnection(connectionString);
      conn.Open();
      var result = await conn.ExecuteQueryAsync<object>("SELECT * FROM Security");
      conn.Close();
      if (result.Any())
      {
        return true;
      }
      return false;
    }
  }
}
