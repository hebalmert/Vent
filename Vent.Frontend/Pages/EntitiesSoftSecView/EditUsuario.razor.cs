using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class EditUsuario
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Usuario? Usuario;

    private FormUsuario? FormUsuario { get; set; }

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Usuario>($"/api/usuarios/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/usuarios");
            return;
        }
        Usuario = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync("/api/usuarios", Usuario);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
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