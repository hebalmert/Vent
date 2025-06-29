namespace Vent.Contracts.Events;

public class UserActivatedEvent
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = default!;
    public DateTime ActivatedAt { get; set; }
}