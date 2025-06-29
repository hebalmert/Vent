using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class CreateUsuarioRole
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private UsuarioRole UsuarioRole = new();

    private FormUsuarioRole? FormUsuarioRole { get; set; }

    [Parameter] public int Id { get; set; }

    private async Task Create()
    {
        UsuarioRole.UsuarioId = Id;
        var responseHttp = await _repository.PostAsync("/api/usuarioRoles", UsuarioRole);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/usuarios");
            return;
        }
        FormUsuarioRole!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/usuarios/details/{Id}");
    }

    private void Return()
    {
        FormUsuarioRole!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/usuarios/details/{Id}");
    }
}