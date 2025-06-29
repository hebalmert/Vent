using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.SellsView;

public partial class CreateSellDetails
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private SellDetails SellDetails = new();

    private FormSellDetails? FormSellDetails { get; set; }
    [Parameter] public int Id { get; set; }  //SellsId

    private string BaseUrl = "/api/sellsDetails";
    private string BaseView = "/sells/details";

    private async Task Create()
    {
        SellDetails.SellId = Id;
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", SellDetails);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}/{Id}");
            return;
        }

        FormSellDetails!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}/{Id}");
    }

    private void Return()
    {
        FormSellDetails!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}/{Id}");
    }
}