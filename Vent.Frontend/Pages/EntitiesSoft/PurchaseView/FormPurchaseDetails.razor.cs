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

namespace Vent.Frontend.Pages.EntitiesSoft.PurchaseView;

public partial class FormPurchaseDetails
{
    private EditContext _editContext = null!;

    private Category? SelectedCategory;
    private List<Category>? Categories;

    private Product? SelectedProduct;
    private List<Product>? Products = new();

    private Product? ItemProducto;
    private decimal Total;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public PurchaseDetail PurchaseDetail { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(PurchaseDetail);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategory();
        if (IsEditControl)
        {
            await LoadProducts(PurchaseDetail.CategoryId);
        }
    }

    private async Task LoadCategory()
    {
        var responseHTTP = await _repository.GetAsync<List<Category>>($"api/categories/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/purchases");
            return;
        }

        Categories = responseHTTP.Response;
        if (IsEditControl)
        {
            SelectedCategory = Categories!.Where(x => x.CategoryId == PurchaseDetail.CategoryId)
                .Select(x => new Category { CategoryId = x.CategoryId, CategoryName = x.CategoryName }).FirstOrDefault();
        }
    }

    private async Task CategoryChanged(Category modelo)
    {
        PurchaseDetail.CategoryId = modelo.CategoryId;
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
            _navigationManager.NavigateTo("/purchases");
            return;
        }

        Products = responseHTTP.Response;
        if (IsEditControl)
        {
            SelectedProduct = Products!.Where(x => x.ProductId == PurchaseDetail.ProductId)
                .Select(x => new Product { ProductId = x.ProductId, ProductName = x.ProductName }).FirstOrDefault();
            Total = PurchaseDetail.SubTotal;
        }
    }

    private async Task ProductsChanged(Product modelo)
    {
        PurchaseDetail.ProductId = modelo.ProductId;
        SelectedProduct = modelo;

        //Traerme el dato del producto
        var responseHTTP = await _repository.GetAsync<Product>($"api/products/{modelo.ProductId}");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/purchases");
            return;
        }

        ItemProducto = responseHTTP.Response;
        //Igualamos datos
        PurchaseDetail.RateTax = ItemProducto!.Tax!.Rate;
        if (PurchaseDetail.RateTax == 0)
        {
            if (ItemProducto.Costo > 0)
            {
                PurchaseDetail.UnitCost = ItemProducto.Costo;
                PurchaseDetail.Quantity = 1;
                Total = (decimal)(PurchaseDetail.UnitCost * PurchaseDetail.Quantity);
            }
        }
        else
        {
            decimal impuesto = ItemProducto!.Tax!.Rate;
            decimal costo = ItemProducto.Costo;
            decimal Precio = costo / ((impuesto / 100) + 1);
            PurchaseDetail.UnitCost = Precio;
            PurchaseDetail.Quantity = 1;
            Total = (decimal)(Precio * PurchaseDetail.Quantity);
        }
    }

    private void CalculoTotalUnit(decimal valor)
    {
        decimal costo = PurchaseDetail.Quantity;
        if (PurchaseDetail.Quantity > 0 && valor > 0)
        {
            Total = (costo * valor);
            PurchaseDetail.UnitCost = valor;
            return;
        }
        return;
    }

    private void CalculoTotalCant(decimal valor)
    {
        decimal costo = PurchaseDetail.UnitCost;
        if (PurchaseDetail.UnitCost > 0 && valor > 0)
        {
            Total = (costo * valor);
            PurchaseDetail.Quantity = valor;
            return;
        }
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
            Title = "Confirmaci�n",
            Text = "�Deseas abandonar la p�gina y perder los cambios?",
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