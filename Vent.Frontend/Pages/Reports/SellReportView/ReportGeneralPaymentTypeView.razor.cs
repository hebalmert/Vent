using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.ReportsDTO;
using Vent.Shared.Responses;

namespace Vent.Frontend.Pages.Reports.SellReportView;

public partial class ReportGeneralPaymentTypeView
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private int CurrentPage = 1;
    private int TotalPages;
    private int PageSize = 15;
    private const string baseUrl = "/api/sells";
    private RepDatePaymentDTO RepDatePaymentDTO = new();

    public List<Sell>? Sells = new();

    private async Task Cargar(int page = 1)
    {
        var url = $"{baseUrl}/ReporteSellDates?Id={RepDatePaymentDTO.PeymentTypeId}& DateStart={Convert.ToString(RepDatePaymentDTO.DateInicio)}&DateEnd={Convert.ToString(RepDatePaymentDTO.DateFin)}";
        var responseHttp = await _repository.GetAsync<List<Sell>>(url);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/");
            return;
        }

        Sells = responseHttp.Response;
    }

    private async Task DownloadExcel()
    {
        if (RepDatePaymentDTO.PeymentTypeId == 0 || Sells!.Count == 0)
        {
            await _sweetAlert.FireAsync("Error", "No hay registros para Exportar", SweetAlertIcon.Warning);
            return;
        }

        var url = $"{baseUrl}/ExportToExcel?Id={RepDatePaymentDTO.PeymentTypeId}&DateStart={RepDatePaymentDTO.DateInicio:yyyy-MM-dd}&DateEnd={RepDatePaymentDTO.DateFin:yyyy-MM-dd}";

        var responseHttp = await _repository.GetFileAsync(url);
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandled)
        {
            await _sweetAlert.FireAsync("Error", "No se pudo descargar el archivo", "error");
            return;
        }
        var fileBytes = responseHttp.Response;
        var fileName = "ReporteVentas.xlsx";
        var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        // Convertir a Base64
        var base64Data = Convert.ToBase64String(fileBytes!);

        // Llamar a JavaScript para descargar
        await JS.InvokeVoidAsync("downloadFile", fileName, fileType, base64Data);
    }

    private async Task SelectedPage(int page)
    {
        CurrentPage = page;
        await Cargar(page);
    }

    private async Task Create()
    {
        if (RepDatePaymentDTO.PeymentTypeId == 0)
        {
            await _sweetAlert.FireAsync("Error", "Debe Seleccionar un Tipo de Pago", "error");
            return;
        }
        await Cargar();
    }

    private void Return()
    {
        _navigationManager.NavigateTo("/");
    }
}