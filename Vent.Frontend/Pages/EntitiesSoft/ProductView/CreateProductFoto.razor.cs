using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.Frontend.Pages.EntitiesViews.ManagerView;
using Vent.AccessService.Repositories;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoft;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.ProductView;

public partial class CreateProductFoto
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private ProductImage ProductImage = new();

    private FormProductoFoto? FormProductoFoto { get; set; }

    [Parameter] public int Id { get; set; }  //ProductId

    private async Task Create()
    {
        ProductImage.ProductId = Id;
        var responseHttp = await _repository.PostAsync("/api/products/postProductimage", ProductImage);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"/products/detailProductPhotos/{Id}");
            return;
        }
        FormProductoFoto!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/products/detailProductPhotos/{Id}");
    }

    private void Return()
    {
        FormProductoFoto!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"/products/detailProductPhotos/{Id}");
    }
}