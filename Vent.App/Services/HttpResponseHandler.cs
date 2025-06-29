using System.Net;
using Vent.AccessService.Repositories;

namespace Vent.App.Services;

public class HttpResponseHandler : IHttpResponseHandler
{
    private readonly IServiceProvider _serviceProvider;

    public HttpResponseHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> HandleErrorAsync<T>(HttpResponseWrapper<T> responseHttp)
    {
        if (responseHttp.HttpResponseMessage == null) return false; // No hay respuesta HTTP

        var statusCode = responseHttp.HttpResponseMessage.StatusCode;
        string title = "Error";
        string? message = string.Empty;

        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                message = "Debe loguearse de nuevo.";
                var _loginService = _serviceProvider.GetRequiredService<ILoginService>();
                await _loginService.LogoutAsync();
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.GoToAsync("//LoginPage"));
                break;

            case HttpStatusCode.Forbidden:
                message = "No tienes permisos para acceder a este recurso.";
                break;

            case HttpStatusCode.NotFound:
                message = "Registro no encontrado.";
                break;

            case HttpStatusCode.InternalServerError:
                message = "Error interno del servidor. Intenta más tarde.";
                break;

            case HttpStatusCode.BadRequest:
                message = await responseHttp.GetErrorMessageAsync();
                message = $"Solicitud incorrecta: {message}";
                break;

            case HttpStatusCode.GatewayTimeout:
                message = "El servidor no respondió a tiempo. Intenta más tarde.";
                break;

            case HttpStatusCode.ServiceUnavailable:
                message = "El servicio no está disponible temporalmente.";
                break;

            case HttpStatusCode.BadGateway:
                message = "El servidor de respaldo no respondió correctamente.";
                break;

            case HttpStatusCode.RequestTimeout:
                message = "La solicitud ha tardado demasiado tiempo.";
                break;

            case HttpStatusCode.UnprocessableEntity:
                message = "Los datos enviados no son válidos. Verifica la información ingresada.";
                break;

            default:
                var messageError = await responseHttp.GetErrorMessageAsync();
                if (messageError != null)
                {
                    message = "Ocurrió un error inesperado."; ;
                }
                break;
        }

        if (!string.IsNullOrEmpty(message))
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Application.Current.MainPage.DisplayAlert(title, message, "OK"));
            return true;
        }

        return false;
    }
}