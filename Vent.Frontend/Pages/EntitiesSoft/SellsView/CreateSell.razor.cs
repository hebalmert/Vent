using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.SellsView;

public partial class CreateSell
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Sell Sell = new();

    private FormSell? FormSell { get; set; }

    private string BaseUrl = "/api/sells";
    private string BaseView = "/sells";

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync<Sell, Sell>($"{BaseUrl}", Sell);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }

        Sell = responseHttp.Response!;
        FormSell!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}/details/{Sell.SellId}");
    }

    private void Return()
    {
        FormSell!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}