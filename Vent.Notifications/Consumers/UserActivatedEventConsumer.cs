using MassTransit;
using Vent.Contracts.Events;
using Vent.Notifications.Services;

namespace Vent.Notifications.Consumers;

public class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly IWelcomeEmailSender _welcomeEmailSender;

    public UserActivatedEventConsumer(IWelcomeEmailSender welcomeEmailSender)
    {
        _welcomeEmailSender = welcomeEmailSender;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var message = context.Message;

        Console.WriteLine($"🔔 Usuario activado: {message.Email} (ID: {message.UserId}) en {message.ActivatedAt}");

        // Aquí podrías enviar una notificación, email, o registrar una actividad

        var result = await _welcomeEmailSender.SendAsync(message.Email, message.FullName!);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"❌ Falló el envío del correo de bienvenida: {result.Message}");
        }
        else
        {
            Console.WriteLine($"📬 Correo de bienvenida enviado a {message.Email}");
        }
    }
}