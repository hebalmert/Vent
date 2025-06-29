namespace Vent.Shared.DTOs;

public class TransferStockDTO
{
    public int TransferId { get; set; }

    public int ProductId { get; set; }

    public decimal DiponibleOrigen { get; set; }
}