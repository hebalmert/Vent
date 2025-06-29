using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Pages.EntitiesSoft.ProductView;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.PurchaseView;

public partial class EditPurchases
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private string BaseUrl = "api/purchases";
    private string BaseView = "/purchases";
    private Purchase? Purchase;

    private FormPurchase? FormPurchase { get; set; }

    [Parameter]
    public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Purchase>($"{BaseUrl}/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        Purchase = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync($"{BaseUrl}", Purchase);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
    }

    private void Return()
    {
        FormPurchase!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}