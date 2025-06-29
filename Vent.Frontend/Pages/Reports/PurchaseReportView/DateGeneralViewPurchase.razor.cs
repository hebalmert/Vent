using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.ReportsDTO;

namespace Vent.Frontend.Pages.Reports.PurchaseReportView;

public partial class DateGeneralViewPurchase
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private int CurrentPage = 1;
    private int TotalPages;
    private int PageSize = 15;
    private const string baseUrl = "/api/purchases/ReportePurchaseDates";
    private RepDateDTO RepDateDTO = new();

    public List<Purchase>? Purchases = new();

    private async Task Cargar(int page = 1)
    {
        var url = $"{baseUrl}?DateStart={Convert.ToString(RepDateDTO.DateInicio)}&DateEnd={Convert.ToString(RepDateDTO.DateFin)}";
        var responseHttp = await _repository.GetAsync<List<Purchase>>(url);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/");
            return;
        }

        Purchases = responseHttp.Response;
    }

    private async Task SelectedPage(int page)
    {
        CurrentPage = page;
        await Cargar(page);
    }

    private async Task Create()
    {
        await Cargar();
    }

    private void Return()
    {
        _navigationManager.NavigateTo("/");
    }
}