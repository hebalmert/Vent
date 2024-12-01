using System.ComponentModel.DataAnnotations;
using Vent.Shared.Resources;

namespace Vent.Shared.Entities;

public class Country
{
    [Key]
    public int CountryId { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Resource))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Name { get; set; } = null!;

    [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} Caracter")]
    [Display(Name = "Cod Phone")]
    public string? CodPhone { get; set; }

    //Propiedad Virtual de Consulta
    [Display(Name = "Estado/Departamentos")]
    public int StatesNumber => States == null ? 0 : States.Count;

    public ICollection<State>? States { get; set; }
}