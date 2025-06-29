using Vent.Shared.Entities;

namespace Vent.Shared.EntitiesSoft;

public class Register
{
    public int RegisterId { get; set; }

    public int RegPurchase { get; set; }

    public int RegSells { get; set; }

    public int RegTransfer { get; set; }

    //A que Corporacion Pertenece
    public int CorporationId { get; set; }

    public Corporation? Corporation { get; set; }
}