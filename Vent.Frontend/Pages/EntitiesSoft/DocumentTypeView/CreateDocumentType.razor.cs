using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;

namespace Vent.Frontend.Pages.EntitiesSoft.DocumentTypeView;

public partial class CreateDocumentType
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private DocumentType DocumentType = new();

    private FormDocumentType? FormDocumentType { get; set; }

    private string BaseUrl = "/api/documentTypes";
    private string BaseView = "/documentTypes";

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", DocumentType);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
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