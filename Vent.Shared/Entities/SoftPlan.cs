using System.ComponentModel.DataAnnotations;

namespace Vent.Shared.Entities;

public class SoftPlan
{
    [Key]
    public int SoftPlanId { get; set; }

    public string Name { get; set; } = null!;

    public int CountItem { get; set; }

    public decimal Price { get; set; }

    public int TimeMonth { get; set; }

    public bool Active { get; set; }
}