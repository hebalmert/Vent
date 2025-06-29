using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vent.Helpers;
using Vent.Notifications.Consumers;
using Vent.Notifications.Services;
using Vent.Shared.ResponsesSec;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // ✅ Configuración SendGrid consumo en local
        services.Configure<SendGridSettings>(context.Configuration.GetSection("SendGrid"));

        // 📨 Servicios de correo
        services.AddScoped<IEmailHelper, EmailHelper>();
        services.AddScoped<IWelcomeEmailSender, WelcomeEmailSender>();

        // 🐇 MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserActivatedEventConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h => { });

                cfg.ReceiveEndpoint("user-activated-event-queue", e =>
                {
                    e.ConfigureConsumer<UserActivatedEventConsumer>(ctx);
                });
            });
        });
    })
    .Build()
    .Run();