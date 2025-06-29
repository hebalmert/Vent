using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.ReportsDTO;

public class RepDatePaymentDTO : RepDateDTO
{
    [Display(Name = "Tipo Pago")]
    public int PeymentTypeId { get; set; }
}