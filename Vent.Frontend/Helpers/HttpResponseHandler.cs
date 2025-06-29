using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.Net;
using Vent.AccessService.Repositories;
using Vent.Frontend.AuthenticationProviders;

namespace Vent.Frontend.Helpers;

public class HttpResponseHandler
{
    private readonly ILoginService _loginService;
    private readonly NavigationManager _navigationManager;
    private readonly SweetAlertService _sweetAlert;

    public HttpResponseHandler(ILoginService loginService,
        NavigationManager navigationManager,
        SweetAlertService sweetAlert)
    {
        _loginService = loginService;
        _navigationManager = navigationManager;
        _sweetAlert = sweetAlert;
    }

    public async Task<bool> HandleErrorAsync<T>(HttpResponseWrapper<T> responseHttp)
    {
        if (responseHttp.HttpResponseMessage == null) return false; // No hay respuesta HTTP

        var statusCode = responseHttp.HttpResponseMessage.StatusCode;

        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                await _sweetAlert.FireAsync("Error", "Debe Loguearse de Nuevo", SweetAlertIcon.Error);
                await _loginService.LogoutAsync();
                _navigationManager.NavigateTo($"/");
                return true;

            case HttpStatusCode.Forbidden:
                await _sweetAlert.FireAsync("Error", "No tienes permisos para acceder a este recurso", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.NotFound:
                await _sweetAlert.FireAsync("Error", "Registro No Encontrado", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.InternalServerError:
                await _sweetAlert.FireAsync("Error", "Error interno del servidor. Intenta más tarde.", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.BadRequest:
                var badRequestMessage = await responseHttp.GetErrorMessageAsync();
                await _sweetAlert.FireAsync("Error", $"Solicitud incorrecta: {badRequestMessage}", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.GatewayTimeout:
                await _sweetAlert.FireAsync("Error", "El servidor no respondió a tiempo. Intenta más tarde.", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.ServiceUnavailable:
                await _sweetAlert.FireAsync("Error", "El servicio no está disponible temporalmente. Intenta más tarde.", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.BadGateway:
                await _sweetAlert.FireAsync("Error", "El servidor de respaldo no respondió correctamente. Intenta más tarde.", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.RequestTimeout:
                await _sweetAlert.FireAsync("Error", "La solicitud ha tardado demasiado tiempo. Intenta más tarde.", SweetAlertIcon.Error);
                return true;

            case HttpStatusCode.UnprocessableEntity:
                await _sweetAlert.FireAsync("Error", "Los datos enviados no son válidos. Verifica la información ingresada.", SweetAlertIcon.Error);
                return true;

            default:
                var messageError = await responseHttp.GetErrorMessageAsync();
                if (messageError != null)
                {
                    await _sweetAlert.FireAsync("Error", messageError, SweetAlertIcon.Error);
                    return true;
                }
                return false;
        }
    }
}