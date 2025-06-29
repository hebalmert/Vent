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

[Route("api/sellsDetails")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class SellDetailsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public SellDetailsController(DataContext context, IFileStorage fileStorage,
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
    public async Task<ActionResult<IEnumerable<SellDetails>>> GetSellDetails([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.SellDetails.Where(x => x.CorporationId == user.CorporationId && x.SellId == pagination.Id)
            .Include(x => x.Product).Include(x => x.Category).AsQueryable();

        //if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //{
        //    queryable = queryable.Where(x => x.TaxName!.ToLower().Contains(pagination.Filter.ToLower()));
        //}

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.SellDetailsId).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SellDetails>> GetSellDetails(int id)
    {
        try
        {
            var modelo = await _context.SellDetails
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
    public async Task<IActionResult> PutSellDetails(SellDetails modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();
            _context.SellDetails.Update(modelo);
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
    public async Task<ActionResult<Tax>> PostSellDetails(SellDetails modelo)
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

            var BuscarItem = await _context.SellDetails.FirstOrDefaultAsync(x => x.SellId == modelo.SellId && x.ProductId == modelo.ProductId);
            if (BuscarItem == null)
            {
                _context.SellDetails.Add(modelo);
            }
            else
            {
                BuscarItem.Quantity += modelo.Quantity;
                BuscarItem.UnitCost = modelo.UnitCost;
                _context.SellDetails.Update(BuscarItem);
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetSellDetails", new { id = modelo.SellDetailsId }, modelo);
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

    [HttpPost("CerrarSells")]
    public async Task<IActionResult> PosCerrarSells(Sell modelo)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

        //Vemos cuantos Items hay en la compra por PurchaseDetails
        var selldetails = await _context.SellDetails.Where(x => x.SellId == modelo.SellId).ToListAsync();
        if (selldetails.Count == 0)
        { return BadRequest("No Existe ningun Item para poder hacer un Cierre de Compra, Agregue Item o Elimine la Compra"); }

        var transaction = await _context.Database.BeginTransactionAsync();
        foreach (var item in selldetails)
        {
            //Actualizamos los inventarios segun la bodega venga en el Modelo
            var ProductStocks = await _context.ProductStocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.ProductStorageId == modelo.ProductStorageId);
            if (ProductStocks == null)
            {
                return BadRequest("No Existe Disponibilidad de Inventarios");
            }
            else
            {
                decimal NuevoStock = (decimal)(ProductStocks.Stock - item.Quantity);
                ProductStocks.Stock = NuevoStock;
                _context.ProductStocks.Update(ProductStocks);
            }
            await _context.SaveChangesAsync();
        }
        //Cambiamos el estatus del Sells para ya no se pueda editar o borrar.
        var UpdateSells = await _context.Sells.FirstOrDefaultAsync(x => x.SellId == modelo.SellId);
        if (UpdateSells == null)
        {
            return BadRequest("Error en la Actualizacion del Estado de Venta, no se pudo Guradar Nada");
        }
        var DatoCliente = await _context.Clients.Where(x => x.ClientId == modelo.ClientId).Include(x => x.DocumentType).FirstOrDefaultAsync();
        UpdateSells.FullName = DatoCliente!.FullName;
        UpdateSells.FullDocumento = $"{DatoCliente.DocumentType!.DocumentName}:{DatoCliente.NroDocument}";
        UpdateSells.FullTelefono = DatoCliente.PhoneNumber;
        UpdateSells.Direccion = DatoCliente.Address;
        UpdateSells.Status = SellType.Procesando;
        _context.Sells.Update(UpdateSells);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok();
    }

    // DELETE: api/Corporations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSellDetails(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.SellDetails.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.SellDetails.Remove(DataRemove);
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