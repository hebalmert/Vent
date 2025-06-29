using System.IdentityModel.Tokens.Jwt;
using Vent.AccessService.Repositories;
using Vent.Shared.ResponsesSec;

namespace Vent.App.Services;

public class LoginService : ILoginService
{
    private readonly IRepository _repository;
    private readonly IHttpResponseHandler _httpResponseHandler;

    public LoginService(IRepository repository, IHttpResponseHandler httpResponseHandler)
    {
        _repository = repository;
        _httpResponseHandler = httpResponseHandler;
    }

    public async Task<bool> LoginAsync(LoginDTO loginDTO)
    {
        // Verificar conexión a Internet
        if (!StatusConnection.IsInternetAvailable())
        {
            await Shell.Current.DisplayAlert("Error", "No hay conexión a Internet.", "OK");
            return false;
        }

        // Realizar la petición al API
        var responseHttp = await _repository.PostAsync<LoginDTO, TokenDTO>("/api/accounts/Login", loginDTO);
        if (await _httpResponseHandler.HandleErrorAsync(responseHttp))
        {
            return false;
        }

        var ResponToken = responseHttp.Response;

        // Guardar el token de forma segura
        await SecureStorage.SetAsync("token", ResponToken!.Token);
        await SecureStorage.SetAsync("DateToken", ResponToken.Expiration.ToString("yyyy-MM-dd HH:mm:ss"));

        // Procesar los claims del token
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(ResponToken.Token) as JwtSecurityToken;
        if (jsonToken != null)
        {
            foreach (var claim in jsonToken.Claims)
            {
                await SecureStorage.SetAsync(claim.Type, claim.Value);
            }
        }

        // Guardar estado de sesión
        await SessionManager.SetLoggedIn(true);

        // Habilitar el menú lateral
        AppShell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        await Shell.Current.GoToAsync("//HomePage");

        return true;
    }

    public async Task LogoutAsync()
    {
        // Eliminar datos de sesión
        SecureStorage.Remove("token");
        SecureStorage.Remove("DateToken");

        // Eliminar claims
        await SessionManager.SetLoggedIn(false);

        // Deshabilitar el menú lateral
        AppShell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;

        // Redirigir al login
        await Shell.Current.GoToAsync("//LoginPage");
    }
}