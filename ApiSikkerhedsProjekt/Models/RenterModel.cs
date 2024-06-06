namespace ApiSikkerhedsProjekt.Models
{
  public class RenterModel
  {
    public required int RenterId { get; set; }

    public required int ApartmentId { get; set; }

    public required int ApartmentComplexId { get; set; }

    public required string Name { get; set; }

    public required string Address { get; set; }
  }
}
