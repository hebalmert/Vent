using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.ManagerView;

public partial class CreateManager
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Manager Manager = new();

    private FormManager? FormManager { get; set; }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync("/api/managers", Manager);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/managers");
            return;
        }
        FormManager!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/managers");
    }

    private void Return()
    {
        FormManager!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/managers");
    }
}