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
using Vent.Shared.ReportsDTO;

namespace Vent.Backend.Controllers.EntitiesSoft;

[Route("api/purchases")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class PurchasesController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public PurchasesController(DataContext context, IFileStorage fileStorage,
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
    [HttpGet("ReportePurchaseDates")]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetReporteSellDates([FromQuery] ReportDataDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        DateTime dateInicio = Convert.ToDateTime(pagination.DateStart);
        DateTime dateFin = Convert.ToDateTime(pagination.DateEnd);

        var queryable = await _context.Purchases.Where(x => x.CorporationId == user.CorporationId && x.Status == PurchaseStatus.Completado
        && x.PurchaseDate >= dateInicio && x.PurchaseDate <= dateFin)
            .Include(x => x.Supplier).Include(x => x.ProductStorage).Include(x => x.PurchaseDetails).ToListAsync();

        return queryable.OrderBy(x => x.PurchaseDate).ToList();
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.Purchases.Where(x => x.CorporationId == user.CorporationId).Include(x => x.ProductStorage)
            .Include(x => x.Supplier).Include(x => x.ProductStorage).Include(x => x.PurchaseDetails).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Supplier!.Name!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.Supplier!.Name).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Purchase>> GetPurchase(int id)
    {
        try
        {
            var modelo = await _context.Purchases.Include(x => x.PurchaseDetails).Include(x => x.Supplier).Include(x => x.ProductStorage)
            .FirstOrDefaultAsync(x => x.PurchaseId == id);

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
    public async Task<IActionResult> PutPurchase(Purchase modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();

            _context.Purchases.Update(modelo);
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
    public async Task<ActionResult<Purchase>> PostPurchase(Purchase modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();
            modelo.CorporationId = Convert.ToInt32(user.CorporationId);

            //Para LLevar el control de Consecutivos de Compra
            int ControlCompra = 0;
            var CheckRegister = await _context.Registers.FirstOrDefaultAsync(x => x.CorporationId == modelo.CorporationId);
            if (CheckRegister == null)
            {
                Register nReg = new()
                {
                    RegPurchase = 1,
                    RegSells = 0,
                    CorporationId = modelo.CorporationId
                };
                ControlCompra = 1;
                _context.Registers.Add(nReg);
            }
            else
            {
                CheckRegister.RegPurchase += 1;
                ControlCompra = CheckRegister.RegPurchase;
                _context.Registers.Update(CheckRegister);
            }
            await _context.SaveChangesAsync();
            //Fin...

            modelo.NroPurchase = ControlCompra;
            _context.Purchases.Add(modelo);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return Ok(modelo);
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
    public async Task<IActionResult> DeletePurchase(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.Purchases.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.Purchases.Remove(DataRemove);
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