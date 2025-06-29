using System.ComponentModel.DataAnnotations;
using Vent.Shared.EntitiesSoft;

namespace Vent.Shared.Entities;

public class City
{
    [Key]
    public int CityId { get; set; }

    public int StateId { get; set; }

    [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Ciudad")]
    public string Name { get; set; } = null!;

    //Relaciones

    public State? State { get; set; }
    public ICollection<ProductStorage>? ProductStorages { get; set; }
    public ICollection<Supplier>? Suppliers { get; set; }
    public ICollection<Client>? Clients { get; set; }
}