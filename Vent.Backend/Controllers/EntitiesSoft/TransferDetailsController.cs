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

[Route("api/transferDetails")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class TransferDetailsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public TransferDetailsController(DataContext context, IFileStorage fileStorage,
        IConfiguration configuration, IUserHelper userHelper)
    {
        _context = context;
        _fileStorage = fileStorage;
        _configuration = configuration;
        _userHelper = userHelper;
    }

    [HttpGet("loadCombo")]
    public async Task<ActionResult<IEnumerable<Tax>>> GetCorporations()
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var listResult = await _context.Taxes.Where(x => x.Active && x.CorporationId == user.CorporationId).ToListAsync();
        return listResult;
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransferDetails>>> GetSellDetails([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.TransferDetails.Where(x => x.CorporationId == user.CorporationId && x.TransferId == pagination.Id)
            .Include(x => x.Product).Include(x => x.Category).AsQueryable();

        //if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //{
        //    queryable = queryable.Where(x => x.TaxName!.ToLower().Contains(pagination.Filter.ToLower()));
        //}

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.TransferDetailsId).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TransferDetails>> GetTranferDetails(int id)
    {
        try
        {
            var modelo = await _context.TransferDetails
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
    public async Task<IActionResult> PutTransferlDetails(TransferDetails modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();
            _context.TransferDetails.Update(modelo);
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

    // POST: api/Corporations
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TransferDetails>> PostSellDetails(TransferDetails modelo)
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

            //Busco el item en TransferDetail, si Existe lo sumo.
            var BuscarItem = await _context.TransferDetails.FirstOrDefaultAsync(x => x.TransferId == modelo.TransferId && x.ProductId == modelo.ProductId);
            if (BuscarItem == null)
            {
                _context.TransferDetails.Add(modelo);
            }
            else
            {
                BuscarItem.Quantity += modelo.Quantity;
                _context.TransferDetails.Update(BuscarItem);
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetTranferDetails", new { id = modelo.TransferDetailsId }, modelo);
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

    [HttpPost("CerrarTrans")]
    public async Task<IActionResult> PosCerrarSells(Transfer modelo)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

        //Vemos cuantos Items hay en la compra por PurchaseDetails
        var transferdetails = await _context.TransferDetails.Where(x => x.TransferId == modelo.TransferId).ToListAsync();
        if (transferdetails.Count == 0)
        { return BadRequest("No Existe ningun Item para poder hacer un Cierre de Transferencia, Agregue Item o Elimine la Transferencia"); }

        var transaction = await _context.Database.BeginTransactionAsync();
        foreach (var item in transferdetails)
        {
            //Vamos Primero a Restar en la Vieja Bodega
            var ProductStockRest = await _context.ProductStocks
                .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.ProductStorageId == modelo.FromProductStorageId);
            if (ProductStockRest == null)
            {
                return BadRequest("Problemas para Conseguir el Producto en la Bodega de Origen");
            }
            else
            {
                decimal NuevoStock = (decimal)(ProductStockRest.Stock - item.Quantity);
                ProductStockRest.Stock = NuevoStock;
                _context.ProductStocks.Update(ProductStockRest);
            }

            //Vamos Primero a Sumar en la nueva Bodega
            var ProductStockPlus = await _context.ProductStocks
                .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.ProductStorageId == modelo.ToProductStorageId);
            if (ProductStockPlus == null)
            {
                ProductStock Nuevo = new()
                {
                    ProductId = item.ProductId,
                    ProductStorageId = modelo.ToProductStorageId,
                    Stock = item.Quantity,
                    CorporationId = item.CorporationId,
                };
                _context.ProductStocks.Add(Nuevo);
            }
            else
            {
                decimal NuevoStock = (decimal)(ProductStockPlus.Stock + item.Quantity);
                ProductStockPlus.Stock = NuevoStock;
                _context.ProductStocks.Update(ProductStockPlus);
            }
            await _context.SaveChangesAsync();
        }

        //Cambiamos el estatus del Sells para ya no se pueda editar o borrar.
        var UpdateTrans = await _context.Transfers.FirstOrDefaultAsync(x => x.TransferId == modelo.TransferId);
        if (UpdateTrans == null)
        {
            return BadRequest("Error en la Actualizacion del Estado de Venta, no se pudo Guradar Nada");
        }

        UpdateTrans.Status = TransferType.Completado;
        _context.Transfers.Update(UpdateTrans);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok();
    }

    // DELETE: api/Corporations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransDetails(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.TransferDetails.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.TransferDetails.Remove(DataRemove);
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