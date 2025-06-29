using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Shared.Entities;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesViews.CorporationView;

public partial class FormCorporation
{
    private EditContext _editContext = null!;
    private SoftPlan? SelectedSoftplan;
    private SoftPlan? SoftplanDays;
    private List<SoftPlan>? SoftPlans;
    private Country? SelectedCountry;
    private List<Country>? Countries;
    private string? ImageUrl;
    private DateTime? DateMin = new DateTime(2025, 1, 1);

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public Corporation Corporation { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Corporation);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadSoftplan();
        await LoadCountries();
    }

    private async Task LoadSoftplan()
    {
        var responseHTTP = await _repository.GetAsync<List<SoftPlan>>($"api/softplans/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/corporations");
            return;
        }

        SoftPlans = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedSoftplan = SoftPlans!.Where(x => x.SoftPlanId == Corporation.SoftPlanId)
                .Select(x => new SoftPlan { SoftPlanId = x.SoftPlanId, Name = x.Name }).FirstOrDefault();
            ImageUrl = Corporation.ImageFullPath;
        }
        else
        {
            Corporation.DateStart = DateTime.Now;
            Corporation.DateEnd = DateTime.Now;
        }
    }

    private async Task LoadCountries()
    {
        var responseHTTP = await _repository.GetAsync<List<Country>>($"api/countries/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/corporations");
            return;
        }

        Countries = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedCountry = Countries!.Where(x => x.CountryId == Corporation.CountryId)
                .Select(x => new Country { CountryId = x.CountryId, Name = x.Name }).FirstOrDefault();
            ImageUrl = Corporation.ImageFullPath;
        }
    }

    private void DateInicioChanged(DateTime? newDate)
    {
        if (SoftplanDays == null)
        { return; }
        Corporation.DateStart = Convert.ToDateTime(newDate);
        DateTime nuevafecha = Corporation.DateStart.AddMonths(SoftplanDays!.Meses);
        var ndate = nuevafecha.ToString("yyyy-MM-dd");
        Corporation.DateEnd = Convert.ToDateTime(ndate);
    }

    private void DateFinalChanged(DateTime? newDate)
    {
        Corporation.DateEnd = Convert.ToDateTime(newDate);
    }

    private void ImageSelected(string imagenBase64)
    {
        Corporation.ImgBase64 = imagenBase64;
        ImageUrl = null;
    }

    private void CountryChanged(Country modelo)
    {
        Corporation.CountryId = modelo.CountryId;
        SelectedCountry = modelo;
    }

    private void SoftPlanChanged(SoftPlan modelo)
    {
        Corporation.SoftPlanId = modelo.SoftPlanId;
        SelectedSoftplan = modelo;

        SoftplanDays = SoftPlans!.FirstOrDefault(x => x.SoftPlanId == modelo.SoftPlanId);
        Corporation.DateStart = Convert.ToDateTime(DateTime.Now);
        DateTime nuevafecha = Corporation.DateStart.AddMonths(SoftplanDays!.Meses);
        var ndate = nuevafecha.ToString("yyyy-MM-dd");
        Corporation.DateEnd = Convert.ToDateTime(ndate);
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