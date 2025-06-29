using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoftSecView;

public partial class DetailsRoles
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private IDialogService _dialogService { get; set; } = null!;
    [Inject] private SweetAlertService _SweetAlert { get; set; } = null!;

    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private int CurrentPage = 1;
    private int TotalPages;
    private int PageSize = 15;
    private const string baseUrl = "/api/usuarioRoles";

    public Usuario? Usuario { get; set; }
    public List<UsuarioRole>? UsuarioRoles { get; set; }

    [Parameter] public int Id { get; set; }  //Codigo del Usuario
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await Cargar();
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await Cargar();
    }

    private async Task SelectedPage(int page)
    {
        CurrentPage = page;
        await Cargar(page);
    }

    private async Task Cargar(int page = 1)
    {
        var url = $"{baseUrl}?id={Id}&page={page}&recordsnumber={PageSize}";
        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }
        var responseHttpCountry = await _repository.GetAsync<Usuario>($"/api/usuarios/{Id}");
        var responseHttp = await _repository.GetAsync<List<UsuarioRole>>(url);

        TotalPages = int.Parse(responseHttp.HttpResponseMessage.Headers.GetValues("Totalpages").FirstOrDefault()!);

        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/usuarios");
            return;
        }

        Usuario = responseHttpCountry.Response;
        UsuarioRoles = responseHttp.Response;
    }

    private async Task ShowModalAsync()
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
        IDialogReference? dialog;
        var parameters = new DialogParameters
            {
                { "Id", Id }
            };
        dialog = await _dialogService.ShowAsync<CreateUsuarioRole>($"Nuevo Role", parameters, options);

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await Cargar();
        }
    }

    private async Task DeleteAsync(int id)
    {
        var result = await _SweetAlert.FireAsync(new SweetAlertOptions
        {
            Title = "Confirmación",
            Text = "¿Realmente deseas eliminar el registro?",
            Icon = SweetAlertIcon.Question,
            ShowCancelButton = true,
            CancelButtonText = "No",
            ConfirmButtonText = "Si"
        });

        var confirm = string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;
        }

        var responseHttp = await _repository.DeleteAsync($"{baseUrl}/{id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/usuarios");
            return;
        }

        await Cargar();
    }
}