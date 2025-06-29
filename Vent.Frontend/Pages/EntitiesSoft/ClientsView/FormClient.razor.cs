using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoft;
using System.Reflection.Metadata;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.ClientsView;

public partial class FormClient
{
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private EditContext _editContext = null!;
    private string? ImageUrl;

    private DocumentType? SelectedDocument = new DocumentType();
    private List<DocumentType>? Documents = new List<DocumentType>();

    private State? SelectedState = new State();
    private List<State>? States = new List<State>();

    private City? SelectedCity = new City();
    private List<City>? Cities = new List<City>();

    private string BaseView = "/clients";

    [Parameter, EditorRequired] public Client Client { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Client);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadState();
        await LoadDocument();
    }

    private async Task LoadState()
    {
        var responseHTTP = await _repository.GetAsync<List<State>>($"/api/states/loadComboStates");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}"); ;
            return;
        }
        States = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedState = States!.Where(x => x.StateId == Client.StateId)
                .Select(x => new State { StateId = x.StateId, Name = x.Name }).FirstOrDefault();

            ImageUrl = Client.ImageFullPath;
            await LoadCities(Client.StateId);
        }
    }

    private async Task StateChanged(State modelo)
    {
        Client.StateId = modelo.StateId;
        SelectedState = modelo;
        if (Client.StateId != 0)
        {
            await LoadCities(Client.StateId);
        }
    }

    private async Task LoadDocument()
    {
        var responseHTTP = await _repository.GetAsync<List<DocumentType>>($"/api/documentTypes/ComboDocument");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}"); ;
            return;
        }
        Documents = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedDocument = Documents!.Where(x => x.DocumentTypeId == Client.DocumentTypeId)
                .Select(x => new DocumentType { DocumentTypeId = x.DocumentTypeId, Abreviatura = x.Abreviatura }).FirstOrDefault();
        }
    }

    private void DocumentChanged(DocumentType modelo)
    {
        Client.DocumentTypeId = modelo.DocumentTypeId;
        SelectedDocument = modelo;
    }

    private async Task LoadCities(int id)
    {
        var responseHTTP = await _repository.GetAsync<List<City>>($"/api/cities/loadComboCities/{id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"{BaseView}"); ;
            return;
        }
        Cities = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedCity = Cities!.Where(x => x.CityId == Client.CityId)
                .Select(x => new City { CityId = x.CityId, Name = x.Name }).FirstOrDefault();
        }
    }

    private void CitiesChanged(City modelo)
    {
        Client.CityId = modelo.CityId;
        SelectedCity = modelo;
    }

    private void ImageSelected(string imagenBase64)
    {
        Client.ImgBase64 = imagenBase64;
        ImageUrl = null;
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = _editContext.IsModified();

        if (!formWasEdited)
        {
            return;
        }

        if (FormPostedSuccessfully)
        {
            return;
        }

        var result = await _sweetAlert.FireAsync(new SweetAlertOptions
        {
            Title = "Confirmación",
            Text = "¿Deseas abandonar la página y perder los cambios?",
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true
        });

        var confirm = !string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private string GetDisplayName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;
            if (property != null)
            {
                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    return displayAttribute.Name!;
                }
            }
        }
        return "Texto no definido";
    }
}