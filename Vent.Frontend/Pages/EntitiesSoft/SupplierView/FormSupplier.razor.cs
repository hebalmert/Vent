using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.Entities;
using System.Runtime.Versioning;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.SupplierView;

public partial class FormSupplier
{
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private EditContext _editContext = null!;
    private string? ImageUrl;

    private State? SelectedState = new State();
    private List<State>? States = new List<State>();

    private City? SelectedCity = new City();
    private List<City>? Cities = new List<City>();

    [Parameter, EditorRequired] public Supplier Supplier { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Supplier);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadState();
    }

    private async Task LoadState()
    {
        var responseHTTP = await _repository.GetAsync<List<State>>($"/api/states/loadComboStates");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/companies");
            return;
        }
        States = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedState = States!.Where(x => x.StateId == Supplier.StateId)
                .Select(x => new State { StateId = x.StateId, Name = x.Name }).FirstOrDefault();
            ImageUrl = Supplier.ImageFullPath;
            await LoadCities(Supplier.StateId);
        }
    }

    private async Task StateChanged(State modelo)
    {
        Supplier.StateId = modelo.StateId;
        SelectedState = modelo;
        if (Supplier.StateId != 0)
        {
            await LoadCities(Supplier.StateId);
        }
    }

    private async Task LoadCities(int id)
    {
        var responseHTTP = await _repository.GetAsync<List<City>>($"/api/cities/loadComboCities/{id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/companies");
            return;
        }
        Cities = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedCity = Cities!.Where(x => x.CityId == Supplier.CityId)
                .Select(x => new City { CityId = x.CityId, Name = x.Name }).FirstOrDefault();
        }
    }

    private void CitiesChanged(City modelo)
    {
        Supplier.CityId = modelo.CityId;
        SelectedCity = modelo;
    }

    private void ImageSelected(string imagenBase64)
    {
        Supplier.ImgBase64 = imagenBase64;
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