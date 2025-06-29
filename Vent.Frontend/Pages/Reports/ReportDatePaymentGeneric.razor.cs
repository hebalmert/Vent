using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.ReportsDTO;

namespace Vent.Frontend.Pages.Reports;

public partial class ReportDatePaymentGeneric
{
    private DateTime? DateMin = new DateTime(2025, 1, 1);
    private PaymentType? SelectedPaymentType;
    private List<PaymentType>? PaymentTypes;

    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public RepDatePaymentDTO RepDatePaymentDTO { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    protected override void OnInitialized()
    {
        RepDatePaymentDTO.DateInicio = DateTime.Now;
        RepDatePaymentDTO.DateFin = DateTime.Now;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadPaymentTypes();
    }

    private async Task LoadPaymentTypes()
    {
        var responseHTTP = await _repository.GetAsync<List<PaymentType>>($"api/paymentTypes/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/");
            return;
        }

        PaymentTypes = responseHTTP.Response;
    }

    private void PaymentTypesChanged(PaymentType modelo)
    {
        RepDatePaymentDTO.PeymentTypeId = modelo.PaymentTypeId;
        SelectedPaymentType = modelo;
    }

    private void DateInicioChanged(DateTime? newDate)
    {
        RepDatePaymentDTO.DateInicio = Convert.ToDateTime(newDate);
    }

    private void DateFinalChanged(DateTime? newDate)
    {
        RepDatePaymentDTO.DateFin = Convert.ToDateTime(newDate);
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