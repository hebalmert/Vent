using Vent.App.ViewModels;

namespace Vent.App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel _ViewModel)
    {
        InitializeComponent();
        BindingContext = _ViewModel;
    }
}