﻿using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.Entities;

public class Country
{
    [Key]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} Caracter")]
    [Display(Name = "Pais")]
    public string Name { get; set; } = null!;

    [MaxLength(10, ErrorMessage = "El campo {0} no puede tener mas de {1} Caracter")]
    [Display(Name = "Cod Phone")]
    public string? CodPhone { get; set; }

    //Propiedad Virtual de Consulta
    [Display(Name = "Estado/Departamentos")]
    public int StatesNumber => States == null ? 0 : States.Count;

    public ICollection<State>? States { get; set; }
}