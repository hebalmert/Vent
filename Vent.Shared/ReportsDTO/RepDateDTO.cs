using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.ReportsDTO;

public class RepDateDTO
{
    [Required(ErrorMessage = "The field {0} is required")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Desde")]
    public DateTime DateInicio { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Hasta")]
    public DateTime DateFin { get; set; }
}