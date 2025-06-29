using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoftSec;
using Vent.Shared.Enum;

namespace Vent.Shared.EntitiesSoft;

public class Sell
{
    [Key]
    public int SellId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Fecha de Compra")]
    public DateTime SellDate { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Compra#")]
    public int NroSell { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Vendedor")]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Cliente")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Tipo Pago")]
    public int PaymentTypeId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Bodega")]
    public int ProductStorageId { get; set; }

    [Display(Name = "Estado")]
    public SellType? Status { get; set; }

    //Campos Fijos, pero no obligatorios, hasta finalizar la Venta.
    [MaxLength(100, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    public string? FullName { get; set; }

    [MaxLength(100, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    public string? FullDocumento { get; set; }

    [MaxLength(100, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    public string? FullTelefono { get; set; }

    [MaxLength(100, ErrorMessage = "El campo no puede ser mayor a {1} de largo")]
    public string? Direccion { get; set; }

    //Propiedades Virtuales

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Subtotal")]
    public decimal SubTotalCompra => SellDetails == null ? 0 : SellDetails.Sum(x => x.SubTotal);

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Impuesto")]
    public decimal ImpuestoTotalCompra => SellDetails == null ? 0 : SellDetails.Sum(x => x.Impuesto);

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total")]
    public decimal TotalCompra => SellDetails == null ? 0 : SellDetails.Sum(x => x.TotalGeneral);

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public Usuario? Usuario { get; set; }

    public Client? Client { get; set; }

    public PaymentType? PaymentType { get; set; }

    public ProductStorage? ProductStorage { get; set; }

    public ICollection<SellDetails>? SellDetails { get; set; }
}