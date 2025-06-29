using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.Shared.ReportsDTO;

namespace Vent.Frontend.Pages.Reports;

public partial class ReportDateGeneric
{
    private DateTime? DateMin = new DateTime(2025, 1, 1);

    [Parameter, EditorRequired] public RepDateDTO RepDateDTO { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    protected override void OnInitialized()
    {
        RepDateDTO.DateInicio = DateTime.Now;
        RepDateDTO.DateFin = DateTime.Now;
    }

    private void DateInicioChanged(DateTime? newDate)
    {
        RepDateDTO.DateInicio = Convert.ToDateTime(newDate);
    }

    private void DateFinalChanged(DateTime? newDate)
    {
        RepDateDTO.DateFin = Convert.ToDateTime(newDate);
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