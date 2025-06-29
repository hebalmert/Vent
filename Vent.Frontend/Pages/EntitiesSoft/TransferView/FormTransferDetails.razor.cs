using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Vent.AccessService.Repositories;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.DTOs;
using Vent.Frontend.Helpers;

namespace Vent.Frontend.Pages.EntitiesSoft.TransferView;

public partial class FormTransferDetails
{
    private EditContext _editContext = null!;

    private Category? SelectedCategory;
    private List<Category>? Categories;

    private Product? SelectedProduct;
    private List<Product>? Products = new();

    private Product? ItemProducto;
    private decimal Total;

    private TransferStockDTO? TransferStockDTO;

    private decimal StockAvaible;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public TransferDetails TransferDetails { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(TransferDetails);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategory();
        if (IsEditControl)
        {
            await LoadProducts(TransferDetails.CategoryId);
        }
    }

    private async Task LoadCategory()
    {
        var responseHTTP = await _repository.GetAsync<List<Category>>($"api/categories/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/sells");
            return;
        }

        Categories = responseHTTP.Response;
        if (IsEditControl)
        {
            SelectedCategory = Categories!.Where(x => x.CategoryId == TransferDetails.CategoryId)
                .Select(x => new Category { CategoryId = x.CategoryId, CategoryName = x.CategoryName }).FirstOrDefault();
        }
    }

    private async Task CategoryChanged(Category modelo)
    {
        TransferDetails.CategoryId = modelo.CategoryId;
        SelectedCategory = modelo;
        Products = new();
        SelectedProduct = new();
        await LoadProducts(modelo.CategoryId);
    }

    private async Task LoadProducts(int Id) //Recibe la CategoryId
    {
        var responseHTTP = await _repository.GetAsync<List<Product>>($"api/products/loadCombo/{Id}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/sells");
            return;
        }
        Products = responseHTTP.Response;
        if (IsEditControl)
        {
            SelectedProduct = Products!.Where(x => x.ProductId == TransferDetails.ProductId)
                .Select(x => new Product { ProductId = x.ProductId, ProductName = x.ProductName }).FirstOrDefault();
        }
    }

    private async Task ProductsChanged(Product modelo)
    {
        TransferDetails.ProductId = modelo.ProductId;
        SelectedProduct = modelo;

        //Traerme el dato del producto
        var responseHTTP = await _repository.GetAsync<TransferStockDTO>($"api/productStocks/transferStock?TransferId={TransferDetails.TransferId}&ProductId={modelo.ProductId}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo($"/transfers/details/{TransferDetails.TransferId}");
            return;
        }

        TransferStockDTO = responseHTTP.Response;
        //Igualamos datos
        StockAvaible = TransferStockDTO!.DiponibleOrigen;
    }

    private void CalculoTotalCant(decimal valor)
    {
        if (valor > StockAvaible)
        {
            TransferDetails.Quantity = StockAvaible;
            return;
        }
        TransferDetails.Quantity = valor;
        return;
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