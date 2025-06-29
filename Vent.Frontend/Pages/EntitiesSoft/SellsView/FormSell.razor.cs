using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Frontend.Helpers;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.Frontend.Pages.EntitiesSoft.SellsView;

public partial class FormSell
{
    private EditContext _editContext = null!;

    private Usuario? SelectedUsuario;
    private List<Usuario>? Usuarios;

    private Client? SelectedClient;
    private List<Client>? Clientes;

    private PaymentType? SelectedPaymentType;
    private List<PaymentType>? PaymentTypes;

    private ProductStorage? SelectedProductStorage;
    private List<ProductStorage>? ProductStorages;

    private DateTime? DateMin = new DateTime(2020, 1, 1);
    private DateTime? DateStart = DateTime.Now;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public Sell Sell { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Sell);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadUsuarios();
        await LoadClients();
        await LoadPaymentType();
        await LoadProductStorage();
    }

    private void DateSellChanged(DateTime? newDate)
    {
        Sell.SellDate = Convert.ToDateTime(newDate);
    }

    private async Task LoadUsuarios()
    {
        var responseHTTP = await _repository.GetAsync<List<Usuario>>($"api/usuarios/loadUsuarioCachier");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/sells");
            return;
        }

        Usuarios = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedUsuario = Usuarios!.Where(x => x.UsuarioId == Sell.UsuarioId)
                .Select(x => new Usuario { UsuarioId = x.UsuarioId, FullName = x.FullName }).FirstOrDefault();
        }
    }

    private async Task LoadClients()
    {
        var responseHTTP = await _repository.GetAsync<List<Client>>($"api/clients/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/sells");
            return;
        }

        Clientes = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedClient = Clientes!.Where(x => x.ClientId == Sell.ClientId)
                .Select(x => new Client { ClientId = x.ClientId, FullName = x.FullName }).FirstOrDefault();
        }
    }

    private async Task LoadPaymentType()
    {
        var responseHTTP = await _repository.GetAsync<List<PaymentType>>($"api/paymentTypes/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/sells");
            return;
        }

        PaymentTypes = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedPaymentType = PaymentTypes!.Where(x => x.PaymentTypeId == Sell.PaymentTypeId)
                .Select(x => new PaymentType { PaymentTypeId = x.PaymentTypeId, PaymentName = x.PaymentName }).FirstOrDefault();
        }
    }

    private async Task LoadProductStorage()
    {
        var responseHTTP = await _repository.GetAsync<List<ProductStorage>>($"api/productStorages/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/sells");
            return;
        }

        ProductStorages = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedProductStorage = ProductStorages!.Where(x => x.ProductStorageId == Sell.ProductStorageId)
                .Select(x => new ProductStorage { ProductStorageId = x.ProductStorageId, StorageName = x.StorageName }).FirstOrDefault();
        }
    }

    private void ProductStorageChanged(ProductStorage modelo)
    {
        Sell.ProductStorageId = modelo.ProductStorageId;
        SelectedProductStorage = modelo;
    }

    private void ClientChanged(Client modelo)
    {
        Sell.ClientId = modelo.ClientId;
        SelectedClient = modelo;
    }

    private void UsuarioChanged(Usuario modelo)
    {
        Sell.UsuarioId = modelo.UsuarioId;
        SelectedUsuario = modelo;
    }

    private void PaymentChanged(PaymentType modelo)
    {
        Sell.PaymentTypeId = modelo.PaymentTypeId;
        SelectedPaymentType = modelo;
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