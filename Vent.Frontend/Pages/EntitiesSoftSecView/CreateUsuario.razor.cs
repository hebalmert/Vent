using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class CreateUsuario
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Usuario Usuario = new();

    private FormUsuario? FormUsuario { get; set; }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync("/api/usuarios", Usuario);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/usuarios");
            return;
        }
        FormUsuario!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/usuarios");
    }

    private void Return()
    {
        FormUsuario!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/usuarios");
    }
}