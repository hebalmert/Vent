using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.CorporationView;

public partial class CreateCorporation
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Corporation Corporation = new();

    private FormCorporation? FormCorporation { get; set; }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync("/api/corporations", Corporation);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/corporations");
            return;
        }
        FormCorporation!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/corporations");
    }

    private void Return()
    {
        FormCorporation!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo("/corporations");
    }
}