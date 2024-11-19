using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Vent.Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7129") });

ConfigureServices(builder.Services);

await builder.Build().RunAsync();



void ConfigureServices(IServiceCollection services)
{
    //Para Agregar Multilenguaje
    services.AddLocalization();
    //Para Implementar MudBlazor
    services.AddMudServices();
}