using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class ProductImage
{
    [Key]
    public int ProductImageId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Display(Name = "Foto")]
    public string? Photo { get; set; }

    //TODO: Pending to put the correct paths
    [Display(Name = "Foto")]
    public string ImageFullPath => Photo == string.Empty || Photo == null
    ? $"http://ventback.nexxtplanet.net/Images/NoImage.png"
    : $"http://ventback.nexxtplanet.net/Images/ImgProducts/{Photo}";

    //? $"https://spi.nexxtplanet.net/Images/NoImage.png"
    //: $"https://spi.nexxtplanet.net/Images/ImgProducts/{Photo}";

    [NotMapped]
    public string? ImgBase64 { get; set; }

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public Product? Product { get; set; }
}