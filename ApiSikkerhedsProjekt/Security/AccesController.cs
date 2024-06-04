using ApiSikkerhedsProjekt.DatabaseCreation;

namespace ApiSikkerhedsProjekt.Security
{
  public class AccesController(DatabaseHelperForAccessControl accessControl)
  {
    public async Task<bool> CheckAccessToRenter(string apiKey, int renterId)
    {
      return await accessControl.ValidateAccessToRenter(apiKey, renterId);
    }
  }
}
