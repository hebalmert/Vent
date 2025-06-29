using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoftSec;
using Vent.Shared.Enum;

namespace Vent.Shared.EntitiesSoft;

public class Transfer
{
    [Key]
    public int TransferId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Fecha de Compra")]
    public DateTime DateTransfer { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Tranferencia#")]
    public int NroTransfer { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Vendedor")]
    public int UsuarioId { get; set; }

    [MaxLength(50, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Desde")]
    public string? FromStorageName { get; set; }

    public int FromProductStorageId { get; set; }

    [MaxLength(50, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Display(Name = "Hacia")]
    public string? ToStorageName { get; set; }

    public int ToProductStorageId { get; set; }

    [Display(Name = "Estado")]
    public TransferType? Status { get; set; }

    //Relaciones
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public Usuario? Usuario { get; set; }

    public ICollection<TransferDetails>? TransferDetails { get; set; }
}