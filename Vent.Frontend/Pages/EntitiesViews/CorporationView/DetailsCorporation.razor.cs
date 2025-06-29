using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.Entities;

namespace Vent.Frontend.Pages.EntitiesViews.CorporationView;

public partial class DetailsCorporation
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Corporation? Corporation;
    private SoftPlan? SoftPlan;
    private Country? Country;

    private FormCorporation? FormCorporation { get; set; }

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHTTP = await _repository.GetAsync<Corporation>($"/api/corporations/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/corporations");
            return;
        }
        Corporation = responseHTTP.Response;

        var responseHTTP2 = await _repository.GetAsync<SoftPlan>($"/api/softplans/{Corporation!.SoftPlanId}");
        // Centralizamos el manejo de errores
        bool errorHandled2 = await _responseHandler.HandleErrorAsync(responseHTTP2);
        if (errorHandled2)
        {
            _navigationManager.NavigateTo("/corporations");
            return;
        }
        SoftPlan = responseHTTP2.Response;

        var responseHTT3 = await _repository.GetAsync<Country>($"/api/countries/{Corporation.CountryId}");
        // Centralizamos el manejo de errores
        bool errorHandled3 = await _responseHandler.HandleErrorAsync(responseHTT3);
        if (errorHandled3)
        {
            _navigationManager.NavigateTo("/corporations");
            return;
        }
        Country = responseHTT3.Response;
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