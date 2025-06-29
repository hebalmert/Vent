using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.DocumentTypeView;

public partial class EditDocumentType
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private string BaseUrl = "api/documentTypes";
    private string BaseView = "/documentTypes";

    private DocumentType? DocumentType;

    private FormDocumentType? FormDocumentType { get; set; }

    [Parameter]
    public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<DocumentType>($"{BaseUrl}/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        DocumentType = responseHTTP.Response;
    }

    private async Task Edit()
    {
        var responseHTTP = await _repository.PutAsync($"{BaseUrl}", DocumentType);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        FormDocumentType!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }

    private void Return()
    {
        FormDocumentType!.FormPostedSuccessfully = true;
        _navigationManager.NavigateTo($"{BaseView}");
    }
}