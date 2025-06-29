using Vent.Shared.Responses;

namespace Vent.Notifications.Services;

public interface IWelcomeEmailSender
{
    Task<Response> SendAsync(string email, string fullName);
}