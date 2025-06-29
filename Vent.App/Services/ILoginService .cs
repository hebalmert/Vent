using Vent.Shared.ResponsesSec;

namespace Vent.App.Services;

public interface ILoginService
{
    Task<bool> LoginAsync(LoginDTO loginDTO);

    Task LogoutAsync();
}