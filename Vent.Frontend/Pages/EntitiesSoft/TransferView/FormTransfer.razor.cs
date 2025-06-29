using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.Enum;
using Vent.Shared.EntitiesSoftSec;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.TransferView;

public partial class FormTransfer
{
    private EditContext _editContext = null!;

    private Usuario? SelectedUsuario;
    private List<Usuario>? Usuarios;

    private ProductStorage? SelectedProductStorage1;
    private List<ProductStorage>? ProductStorages1;

    private ProductStorage? SelectedProductStorage2;
    private List<ProductStorage>? ProductStorages2;

    private DateTime? DateMin = new DateTime(2024, 1, 1);
    private DateTime? DateStart = DateTime.Now;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public Transfer Transfer { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Transfer);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadUsuarios();
        await LoadProductStorage1();
        await LoadProductStorage2();
    }

    private void DateTransferChanged(DateTime? newDate)
    {
        Transfer.DateTransfer = Convert.ToDateTime(newDate);
    }

    private async Task LoadUsuarios()
    {
        var responseHTTP = await _repository.GetAsync<List<Usuario>>($"api/usuarios/loadUsuarioStorage");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/transfers");
            return;
        }

        Usuarios = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedUsuario = Usuarios!.Where(x => x.UsuarioId == Transfer.UsuarioId)
                .Select(x => new Usuario { UsuarioId = x.UsuarioId, FullName = x.FullName }).FirstOrDefault();
        }
    }

    private void UsuarioChanged(Usuario modelo)
    {
        Transfer.UsuarioId = modelo.UsuarioId;
        SelectedUsuario = modelo;
    }

    private async Task LoadProductStorage1()
    {
        var responseHTTP = await _repository.GetAsync<List<ProductStorage>>($"api/productStorages/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/transfers");
            return;
        }

        ProductStorages1 = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedProductStorage1 = ProductStorages1!.Where(x => x.ProductStorageId == Transfer.FromProductStorageId)
                .Select(x => new ProductStorage { ProductStorageId = x.ProductStorageId, StorageName = x.StorageName }).FirstOrDefault();
        }
    }

    private void ProductStorageChanged1(ProductStorage modelo)
    {
        Transfer.FromProductStorageId = modelo.ProductStorageId;
        SelectedProductStorage1 = modelo;
    }

    private async Task LoadProductStorage2()
    {
        var responseHTTP = await _repository.GetAsync<List<ProductStorage>>($"api/productStorages/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/transfers");
            return;
        }

        ProductStorages2 = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedProductStorage2 = ProductStorages2!.Where(x => x.ProductStorageId == Transfer.ToProductStorageId)
                .Select(x => new ProductStorage { ProductStorageId = x.ProductStorageId, StorageName = x.StorageName }).FirstOrDefault();
        }
    }

    private void ProductStorageChanged2(ProductStorage modelo)
    {
        Transfer.ToProductStorageId = modelo.ProductStorageId;
        SelectedProductStorage2 = modelo;
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