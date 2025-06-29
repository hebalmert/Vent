using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Categoria")]
    public string CategoryName { get; set; } = null!;

    [Display(Name = "Foto")]
    public string? Photo { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //TODO: Pending to put the correct paths
    [Display(Name = "Foto")]
    public string ImageFullPath => Photo == string.Empty || Photo == null
    ? $"http://ventback.nexxtplanet.net/Images/NoImage.png"
    : $"http://ventback.nexxtplanet.net/Images/ImgCategory/{Photo}";

    //? $"https://spi.nexxtplanet.net/Images/NoImage.png"
    //: $"https://spi.nexxtplanet.net/Images/ImgCategory/{Photo}";

    [NotMapped]
    public string? ImgBase64 { get; set; }

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public ICollection<Product>? Products { get; set; }

    public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }

    public ICollection<SellDetails>? SellDetails { get; set; }

    public ICollection<TransferDetails>? TransferDetails { get; set; }
}