using Vent.App.ViewModels;

namespace Vent.App
{
    public partial class App : Application
    {
        private readonly AppShellViewModel _appShellViewModel;

        public App(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            // Asignar el MainPage
            MainPage = new AppShell(appShellViewModel);
            _appShellViewModel = appShellViewModel;
            GoToInitialPage();
        }

        private async void GoToInitialPage()
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
                await Shell.Current.GoToAsync("//HomePage");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}