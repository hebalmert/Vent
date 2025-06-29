using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vent.Helpers;
using Vent.Shared.Responses;

namespace Vent.Notifications.Services
{
    public class WelcomeEmailSender : IWelcomeEmailSender
    {
        private readonly IEmailHelper _emailHelper;

        public WelcomeEmailSender(IEmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public async Task<Response> SendAsync(string email, string fullName)
        {
            string subject = "¡Bienvenido a Vent!";
            string body = $"Hola {fullName},\n\nTu cuenta ha sido activada exitosamente. ¡Bienvenido a nuestra comunidad!";

            return await _emailHelper.ConfirmarCuenta(email, fullName, subject, body);
        }
    }
}