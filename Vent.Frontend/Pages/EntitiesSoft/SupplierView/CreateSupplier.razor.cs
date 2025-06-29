using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Pages.EntitiesSoft.CategoryView;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.SupplierView;

public partial class CreateSupplier
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Supplier Supplier = new();
    private FormSupplier? FormSupplier { get; set; }

    private string BaseUrl = "/api/suppliers";
    private string BaseView = "/suppliers";

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", Supplier);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        FormSupplier!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }

    private void Return()
    {
        FormSupplier!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}