using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class DocumentType
{
    public int DocumentTypeId { get; set; }

    [MaxLength(100, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Nombre Documento")]
    public string DocumentName { get; set; } = null!;

    [MaxLength(10, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Abreviatura")]
    public string Abreviatura { get; set; } = null!;

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //A que Corporacion Pertenece
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public ICollection<Client>? Clients { get; set; }
}