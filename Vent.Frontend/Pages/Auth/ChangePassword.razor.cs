using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.ResponsesSec;

namespace Vent.Frontend.Pages.Auth;

public partial class ChangePassword
{
    private ChangePasswordDTO changePasswordDTO = new();
    private bool loading;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private async Task ChangePasswordAsync()
    {
        loading = true;
        var responseHttp = await Repository.PostAsync("/api/accounts/changePassword", changePasswordDTO);
        loading = false;
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            NavigationManager.NavigateTo("/");
            return;
        }
        MudDialog.Cancel();
        NavigationManager.NavigateTo("/");
        Snackbar.Add("Su Clave se actualizo con Exito", Severity.Success);
    }

    private void ReturnAction()
    {
        MudDialog.Cancel();
        NavigationManager.NavigateTo("/");
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