using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Vent.Backend.Data;
using Vent.Backend.Data.LoadCountries;
using Vent.DataAccess;
using Vent.Repositories.Implementation;
using Vent.Repositories.Interfaces;
using Vent.Services.Implementation;
using Vent.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Se Agrega el .AddJsonOption para evitar todas las redundancias Ciclicas automaticamente
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Conexion a la base de datos
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=DefaultConnection"));
//Alimentgador de Base de Datos SeedDb
builder.Services.AddTransient<SeedDb>();

//Declarando Dependencias
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

var app = builder.Build();

//Inyeccion del SeeDB
SeedData(app);
void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using var scope = scopedFactory!.CreateScope();
    var service = scope.ServiceProvider.GetService<SeedDb>();
    service!.SeedAsync().Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();