using Vent.App.Services;
using Vent.App.ViewModels;
using Vent.App.Views;

namespace Vent.App;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        CheckLoginStatus();

        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("HomePage", typeof(HomePage));
    }

    private async void CheckLoginStatus()
    {
        bool isLoggedIn = await SessionManager.IsLoggedIn();

        if (!isLoggedIn)
        {
            this.FlyoutBehavior = FlyoutBehavior.Disabled;
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}