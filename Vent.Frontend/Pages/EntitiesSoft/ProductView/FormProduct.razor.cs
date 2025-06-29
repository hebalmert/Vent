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

namespace Vent.Frontend.Pages.EntitiesSoft.ProductView;

public partial class FormProduct
{
    private EditContext _editContext = null!;

    private Category? SelectedCategory;
    private List<Category>? Categories;

    private Tax? SelectedTax;
    private List<Tax>? Taxes;

    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    [Parameter, EditorRequired] public Product Product { get; set; } = null!;
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        _editContext = new(Product);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategories();
        await LoadTaxes();
    }

    private async Task LoadCategories()
    {
        var responseHTTP = await _repository.GetAsync<List<Category>>($"api/categories/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/products");
            return;
        }

        Categories = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedCategory = Categories!.Where(x => x.CategoryId == Product.CategoryId)
                .Select(x => new Category { CategoryId = x.CategoryId, CategoryName = x.CategoryName }).FirstOrDefault();
        }
    }

    private async Task LoadTaxes()
    {
        var responseHTTP = await _repository.GetAsync<List<Tax>>($"api/taxes/loadCombo");
        // Centralizamos el manejo de errores
        bool errorHandled = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandled)
        {
            _navigationManager.NavigateTo("/products");
            return;
        }

        Taxes = responseHTTP.Response;
        if (IsEditControl == true)
        {
            SelectedTax = Taxes!.Where(x => x.TaxId == Product.TaxId)
                .Select(x => new Tax { TaxId = x.TaxId, TaxName = x.TaxName }).FirstOrDefault();
        }
    }

    private void TaxesChanged(Tax modelo)
    {
        Product.TaxId = modelo.TaxId;
        SelectedTax = modelo;
    }

    private void CategoriesChanged(Category modelo)
    {
        Product.CategoryId = modelo.CategoryId;
        SelectedCategory = modelo;
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