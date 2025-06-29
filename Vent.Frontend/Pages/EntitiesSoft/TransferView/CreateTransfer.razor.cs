using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.TransferView;

public partial class CreateTransfer
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Transfer Transfer = new();

    private FormTransfer? FormTransfer { get; set; }

    private string BaseUrl = "/api/transfers";
    private string BaseView = "/transfers";

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync<Transfer, Transfer>($"{BaseUrl}", Transfer);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        Transfer = responseHttp.Response!;
        FormTransfer!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}/details/{Transfer.TransferId}");
    }

    private void Return()
    {
        FormTransfer!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}