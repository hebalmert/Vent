using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Display(Name = "Nombre")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Categoria")]
    public int CategoryId { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "Descripción")]
    [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Costo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Costo { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Impuesto")]
    public int TaxId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Precio Venta Sin Iva")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Price { get; set; }

    [Display(Name = "Seriales")]
    public bool WithSerials { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //Propiedades Virtuales
    public int TotalImagen => ProductImages == null ? 0 : ProductImages.Count();

    public string FotoMuestra => ProductImages == null || ProductImages.Count() == 0 ? $"https://localhost:7148/Images/NoImage.png" : ProductImages.FirstOrDefault()!.ImageFullPath;

    public decimal TotalInventario => ProductStocks == null ? 0 : ProductStocks.Sum(x => x.Stock);

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public Category? Category { get; set; }

    public Tax? Tax { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; }

    public ICollection<ProductStock>? ProductStocks { get; set; }

    public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }

    public ICollection<SellDetails>? SellDetails { get; set; }

    public ICollection<TransferDetails>? TransferDetails { get; set; }
}