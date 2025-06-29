using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Pages.EntitiesSoft.PurchaseView;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.SellsView;

public partial class EditSellDetails
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private SellDetails? SellDetails;
    private FormSellDetails? FormSellDetails { get; set; }

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<SellDetails>($"/api/sellsDetails/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"/sells/details/{Id}");
            return;
        }
        SellDetails = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync("/api/sellsDetails", SellDetails);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"/sells/details/{Id}");
            return;
        }
        FormSellDetails!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/sells/details/{SellDetails!.SellId}");
    }

    private void Return()
    {
        FormSellDetails!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/sells/details/{SellDetails!.SellId}");
    }
}