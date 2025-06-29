using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vent.AccessData.Data;
using Vent.Backend.Helpers;
using Vent.Helpers;
using Vent.Shared.DTOs;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers.EntitiesSoft;

[Route("api/productStocks")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class ProductStocksController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public ProductStocksController(DataContext context, IFileStorage fileStorage,
        IConfiguration configuration, IUserHelper userHelper)
    {
        _context = context;
        _fileStorage = fileStorage;
        _configuration = configuration;
        _userHelper = userHelper;
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductStock>>> GetProductStock([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.ProductStocks.Where(x => x.CorporationId == user.CorporationId && x.ProductId == pagination.Id)
            .Include(x => x.ProductStorage).Include(x => x.Product).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.ProductStorage!.StorageName!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.ProductStorage!.StorageName!).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductStock>> GetProductStock(int id)
    {
        try
        {
            var modelo = await _context.ProductStocks
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

    [HttpGet("transferstock")]
    public async Task<ActionResult<TransferStockDTO>> GetProductStock([FromQuery] TransferStockDTO modelo)
    {
        try
        {
            var bodegaOrigen = await _context.Transfers.FindAsync(modelo.TransferId);
            var stockDisponible = await _context.ProductStocks
                .FirstOrDefaultAsync(x => x.ProductId == modelo.ProductId && x.ProductStorageId == bodegaOrigen!.FromProductStorageId);
            if (stockDisponible == null || stockDisponible.Stock == 0)
            {
                return BadRequest("No Existe Este Producto en esta Bodega o El inventario es igual a 0 (Cero)");
            }
            TransferStockDTO NStock = new()
            {
                ProductId = modelo.ProductId,
                TransferId = modelo.TransferId,
                DiponibleOrigen = stockDisponible!.Stock
            };
            return NStock;
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
}