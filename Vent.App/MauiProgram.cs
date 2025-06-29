using Microsoft.Extensions.Logging;
using Vent.AccessService.Repositories;
using Vent.App.Services;
using Vent.App.ViewModels;
using Vent.App.Views;

namespace Vent.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IRepository>(sp =>
            {
                //var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7148") };
                var httpClient = new HttpClient { BaseAddress = new Uri("http://ventback.nexxtplanet.net") };
                return new Repository(httpClient, async () =>
                {
                    var token = await SecureStorage.GetAsync("token");
                    return token ?? string.Empty; // Retorna un string vacío si el token no está presente
                });
            });
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddSingleton<IHttpResponseHandler, HttpResponseHandler>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<HomePage>();

            return builder.Build();
        }
    }
}