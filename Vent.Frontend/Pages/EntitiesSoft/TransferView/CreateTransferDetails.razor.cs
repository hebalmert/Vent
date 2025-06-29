using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Pages.EntitiesSoft.SellsView;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.TransferView;

public partial class CreateTransferDetails
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private TransferDetails TransferDetails = new();

    private FormTransferDetails? FormTransferDetails { get; set; }
    [Parameter] public int Id { get; set; }  //TransferId

    private string BaseUrl = "/api/transferDetails";
    private string BaseView = "/transfers/details";

    protected override void OnInitialized()
    {
        TransferDetails.TransferId = Id;
    }

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", TransferDetails);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}/{Id}");
            return;
        }
        FormTransferDetails!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}/{Id}");
    }

    private void Return()
    {
        FormTransferDetails!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}/{Id}");
    }
}