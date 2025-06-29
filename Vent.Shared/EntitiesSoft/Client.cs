using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    [Display(Name = "Nombres")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    [Display(Name = "Apellidos")]
    public string LastName { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Tipo Documento")]
    public int DocumentTypeId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [MaxLength(15, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Documento")]
    public string NroDocument { get; set; } = null!;

    [MaxLength(25, ErrorMessage = "El {0} no puede tener mas de {1} Caracteres.")]
    [Display(Name = "Telefono")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "La {0} es Obligatoria")]
    [MaxLength(256, ErrorMessage = "El campo no puede ser mayor a {0} de largo")]
    [Display(Name = "Direccion")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Departamento")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Ciudad")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [MaxLength(256, ErrorMessage = "El campo no puede ser mayor a {0} de largo")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    public string UserName { get; set; } = null!;

    [Display(Name = "Foto")]
    public string? Photo { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //Propiedades Virtuales
    //TODO: Pending to put the correct paths
    [Display(Name = "Foto")]
    public string ImageFullPath => Photo == string.Empty || Photo == null
    ? $"http://ventback.nexxtplanet.net/Images/NoImage.png"
    : $"http://ventback.nexxtplanet.net/Images/ImgClients/{Photo}";

    //? $"https://spi.nexxtplanet.net/Images/NoImage.png"
    //: $"https://spi.nexxtplanet.net/Images/ImgClients/{Photo}";

    [NotMapped]
    public string? ImgBase64 { get; set; }

    //A que Corporacion Pertenece
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public DocumentType? DocumentType { get; set; }

    public State? State { get; set; }

    public City? City { get; set; }

    public ICollection<Sell>? Sells { get; set; }
}