using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vent.AccessData.Data;
using Vent.Backend.Helpers;
using Vent.Helpers;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.Enum;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers.EntitiesSoft;

[Route("api/purchaseDetails")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class PurchaseDetailsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public PurchaseDetailsController(DataContext context, IFileStorage fileStorage,
        IConfiguration configuration, IUserHelper userHelper)
    {
        _context = context;
        _fileStorage = fileStorage;
        _configuration = configuration;
        _userHelper = userHelper;
    }

    [HttpGet("loadComboStatus")]
    public ActionResult<IEnumerable<EnumItemModel>> GetPeriodicidads()
    {
        List<EnumItemModel> list = Enum.GetValues(typeof(PurchaseStatus)).Cast<PurchaseStatus>().Select(c => new EnumItemModel()
        {
            Name = c.ToString(),
            Value = (int)c
        }).ToList();

        list.Insert(0, new EnumItemModel
        {
            Name = "[Seleccione Estado...]",
            Value = 0
        });

        return list;
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseDetail>>> GetPurchaseDetails([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.PurchaseDetails.Where(x => x.CorporationId == user.CorporationId && x.PurchaseId == pagination.Id)
            .Include(x => x.Product).Include(x => x.Category).AsQueryable();

        //if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //{
        //    queryable = queryable.Where(x => x.Supplier!.Name!.ToLower().Contains(pagination.Filter.ToLower()));
        //}

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.PurchaseDetailId).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseDetail>> GetPurchaseDetails(int id)
    {
        try
        {
            var modelo = await _context.PurchaseDetails
            .FindAsync(id);

            if (modelo == null)
            {
                return NotFound();
            }

            return modelo;
        }
        catch (DbUpdateException dbUpdateException)
        {
            return BadRequest(dbUpdateException.InnerException!.Message);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    // PUT: api/Corporations/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut]
    public async Task<IActionResult> PutPurchaseDetails(PurchaseDetail modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();
            _context.PurchaseDetails.Update(modelo);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return Ok();
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
            {
                return BadRequest("Ya existe un Registro con el mismo nombre.");
            }
            else
            {
                return BadRequest(dbUpdateException.InnerException.Message);
            }
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("CerrarPurchase")]
    public async Task<IActionResult> PosCerrarPurchase(Purchase modelo)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

        //Vemos cuantos Items hay en la compra por PurchaseDetails
        var PurchaseDetails = await _context.PurchaseDetails.Where(x => x.PurchaseId == modelo.PurchaseId).ToListAsync();
        if (PurchaseDetails.Count == 0)
        { return BadRequest("No Existe ningun Item para poder hacer un Cierre de Compra, Agregue Item o Elimine la Compra"); }

        var transaction = await _context.Database.BeginTransactionAsync();
        foreach (var item in PurchaseDetails)
        {
            //Actualizamos los inventarios segun la bodega venga en el Modelo
            var ProductStocks = await _context.ProductStocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.ProductStorageId == modelo.ProductStorageId);
            if (ProductStocks == null)
            {
                ProductStock Inventario = new()
                {
                    ProductId = item.ProductId,
                    ProductStorageId = modelo.ProductStorageId,
                    Stock = item.Quantity,
                    CorporationId = modelo.CorporationId
                };
                _context.ProductStocks.Add(Inventario);
            }
            else
            {
                decimal NuevoStock = (decimal)(ProductStocks.Stock + item.Quantity);
                ProductStocks.Stock = NuevoStock;
                _context.ProductStocks.Update(ProductStocks);
            }
            //Actualizamos el producto con su nuevo valor de Costo
            var UpdateProduct = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
            var tasa = (item.RateTax / 100) + 1;
            var costoUnitario = item.UnitCost;
            var NewUniCost = (tasa * costoUnitario);
            UpdateProduct!.Costo = NewUniCost;
            _context.Products.Update(UpdateProduct);

            await _context.SaveChangesAsync();
        }
        //Cambiamos el estatus del Purchas para ya no se pueda editar o borrar.
        var UpdatePurchase = await _context.Purchases.FirstOrDefaultAsync(x => x.PurchaseId == modelo.PurchaseId);
        if (UpdatePurchase == null)
        {
            return BadRequest("Error en la Actualizacion del Estado de Compra, no se pudo Guradar Nada");
        }
        UpdatePurchase.Status = PurchaseStatus.Completado;
        _context.Purchases.Update(UpdatePurchase);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok();
    }

    // POST: api/Corporations
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<PurchaseDetail>> PostPurchaseDetails(PurchaseDetail modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();
            modelo.CorporationId = Convert.ToInt32(user.CorporationId);
            //Guardar el nombre del producto para el Historial
            var nombreProduct = await _context.Products.Where(x => x.ProductId == modelo.ProductId)
                .Select(x => x.ProductName).FirstOrDefaultAsync();
            modelo.NameProduct = nombreProduct;

            var BuscarItem = await _context.PurchaseDetails.FirstOrDefaultAsync(x => x.PurchaseId == modelo.PurchaseId && x.ProductId == modelo.ProductId);
            if (BuscarItem == null)
            {
                _context.PurchaseDetails.Add(modelo);
            }
            else
            {
                BuscarItem.Quantity += modelo.Quantity;
                BuscarItem.UnitCost = modelo.UnitCost;
                _context.PurchaseDetails.Update(BuscarItem);
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetPurchaseDetails", new { id = modelo.PurchaseId }, modelo);
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
            {
                return BadRequest("Ya existe un Registro con el mismo nombre.");
            }
            else
            {
                return BadRequest(dbUpdateException.InnerException.Message);
            }
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    // DELETE: api/Corporations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePurchaseDetails(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.PurchaseDetails.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.PurchaseDetails.Remove(DataRemove);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return NoContent();
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException!.Message.Contains("REFERENCE"))
            {
                return BadRequest("Existen Registros Relacionados y no se puede Eliminar");
            }
            else
            {
                return BadRequest(dbUpdateException.InnerException.Message);
            }
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}