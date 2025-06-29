using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.ProductView;

public partial class CreateProduct
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Product Product = new();

    private FormProduct? FormProduct { get; set; }

    private string BaseUrl = "/api/products";
    private string BaseView = "/products";

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", Product);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        FormProduct!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }

    private void Return()
    {
        FormProduct!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}