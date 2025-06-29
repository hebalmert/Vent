using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vent.App.Services;

namespace Vent.App.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly ILoginService _loginService;

    public AppShellViewModel(ILoginService loginService)
    {
        _loginService = loginService;
    }

    // Comando Logout
    [RelayCommand]
    public async Task LogoutAsync()
    {
        // Realizar el logout
        await _loginService.LogoutAsync();

        // Redirigir al LoginPage después de cerrar sesión
        await Shell.Current.GoToAsync("//LoginPage");

        // Opcional: Puedes deshabilitar el FlyoutBehavior para que no aparezca el menú
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
    }
}