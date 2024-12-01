using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Vent.Frontend;
using Vent.Frontend.Repositories;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7129") });

ConfigureServices(builder.Services);

await builder.Build().RunAsync();

void ConfigureServices(IServiceCollection services)
{
    //Mensajes Sweet tipo Modal
    services.AddSweetAlert2();
    //Para Agregar Multilenguaje
    services.AddLocalization();
    //Para Implementar MudBlazor
    services.AddMudServices();

    services.AddScoped<IRepository, Repository>();
}