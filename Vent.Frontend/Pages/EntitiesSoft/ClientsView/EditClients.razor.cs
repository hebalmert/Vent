using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.ClientsView;

public partial class EditClients
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Client? Client;

    private FormClient? FormClient { get; set; }

    private string BaseUrl = "/api/clients";
    private string BaseView = "/clients";

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Client>($"{BaseUrl}/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        Client = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync($"{BaseUrl}", Client);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        FormClient!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }

    private void Return()
    {
        FormClient!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}