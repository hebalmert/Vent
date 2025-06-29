using System.ComponentModel.DataAnnotations;
using Vent.Shared.EntitiesSoft;

namespace Vent.Shared.Entities;

public class State
{
    [Key]
    public int StateId { get; set; }

    public int CountryId { get; set; }

    [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Depart/Estado")]
    public string Name { get; set; } = null!;

    //Propiedad Virtual
    public int NumCity => Cities == null ? 0 : Cities.Count();

    //Relaciones
    public Country? Country { get; set; }

    public ICollection<City>? Cities { get; set; }

    public ICollection<ProductStorage>? ProductStorages { get; set; }

    public ICollection<Supplier>? Suppliers { get; set; }

    public ICollection<Client>? Clients { get; set; }
}