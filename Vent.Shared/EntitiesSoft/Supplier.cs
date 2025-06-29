using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Nombre del Proveedor")]
    public string Name { get; set; } = null!;

    [MaxLength(15, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "NIT/CCI")]
    public string? NroDocument { get; set; }

    [MaxLength(15, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    [MaxLength(100, ErrorMessage = "El Maximo de caracteres es {0}")]
    [EmailAddress]
    [Display(Name = "Correo Electrónico")]
    public string? Email { get; set; }

    [MaxLength(100, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Contacto")]
    public string? ContactName { get; set; }

    [MaxLength(200, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Dirección")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Departamento")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Ciudad")]
    public int CityId { get; set; }

    [Display(Name = "Foto")]
    public string? Photo { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //TODO: Pending to put the correct paths
    [Display(Name = "Foto")]
    public string ImageFullPath => Photo == string.Empty || Photo == null
    ? $"https://localhost:7148/Images/NoImage.png"
    : $"https://localhost:7148/Images/ImgSuppliers/{Photo}";

    //? $"https://spi.nexxtplanet.net/Images/NoImage.png"
    //: $"https://spi.nexxtplanet.net/Images/ImgSuppliers/{Photo}";

    [NotMapped]
    public string? ImgBase64 { get; set; }

    //A que Corporacion Pertenece

    public int CorporationId { get; set; }
    public Corporation? Corporation { get; set; }

    // Relación

    public State? State { get; set; }
    public City? City { get; set; }
    public ICollection<Purchase>? Purchases { get; set; }
}