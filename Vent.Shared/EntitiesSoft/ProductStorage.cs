﻿using System.ComponentModel.DataAnnotations;
using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class ProductStorage
{
    [Key]
    public int ProductStorageId { get; set; }

    [MaxLength(50, ErrorMessage = "El Maximo de caracteres es {0}")]
    [Required(ErrorMessage = "El campo {0} es Requerido")]
    [Display(Name = "Bodega")]
    public string StorageName { get; set; } = null!;

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Departamento")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "El {0} es Obligatorio")]
    [Display(Name = "Ciudad")]
    public int CityId { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; }

    //A que Corporacion Pertenece
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }

    public State? State { get; set; }

    public City? City { get; set; }

    public ICollection<ProductStock>? ProductStocks { get; set; }

    public ICollection<Purchase>? Purchases { get; set; }

    public ICollection<Sell>? Sells { get; set; }
}