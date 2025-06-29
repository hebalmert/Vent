using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class PaymentType
{
    public int PaymentTypeId { get; set; }

    [MaxLength(50, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Tipo Pago")]
    public string PaymentName { get; set; } = null!;

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //A que Corporacion Pertenece
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public ICollection<Sell>? Sells { get; set; }
}