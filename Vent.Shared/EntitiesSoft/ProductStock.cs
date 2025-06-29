using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class ProductStock
{
    public int ProductStockId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Bodega")]
    public int ProductStorageId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Stock { get; set; }

    // Método para actualizar el stock
    public void AddStock(decimal quantity)
    {
        Stock += quantity;
    }

    public void ReduceStock(decimal quantity)
    {
        if (Stock >= quantity)
            Stock -= quantity;
        else
            throw new InvalidOperationException("No hay suficiente stock disponible.");
    }

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public Product? Product { get; set; }

    public ProductStorage? ProductStorage { get; set; }
}