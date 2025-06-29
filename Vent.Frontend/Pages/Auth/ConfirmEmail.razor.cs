using Microsoft.AspNetCore.Components;
using MudBlazor;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.Auth;

public partial class ConfirmEmail
{
    private string? message;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    protected async Task ConfirmAccountAsync()
    {
        var responseHttp = await Repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            NavigationManager.NavigateTo("/");
            return;
        }
        Snackbar.Add("Su Cuenta ha sido Confirmada", Severity.Success);
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        NavigationManager.NavigateTo("/");
        await DialogService.ShowAsync<Login>("Iniciar Sesion", closeOnEscapeKey);
    }
}