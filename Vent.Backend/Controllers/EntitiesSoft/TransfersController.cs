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

[Route("api/transfers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class TransfersController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public TransfersController(DataContext context, IFileStorage fileStorage,
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
        List<EnumItemModel> list = Enum.GetValues(typeof(TransferType)).Cast<TransferType>().Select(c => new EnumItemModel()
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
    [HttpGet("ReporteTransferDates")]
    public async Task<ActionResult<IEnumerable<Sell>>> GetReporteTransferDates([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        DateTime dateInicio = Convert.ToDateTime(pagination.DateStart);
        DateTime dateFin = Convert.ToDateTime(pagination.DateEnd);

        var queryable = _context.Sells
            .Where(x => x.CorporationId == user.CorporationId && x.SellDate >= dateInicio && x.SellDate <= dateFin)
            .Include(x => x.Client).Include(x => x.Usuario).Include(x => x.ProductStorage).Include(x => x.SellDetails).AsQueryable();

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.SellDate).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transfer>>> GetTransfers([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.Transfers.Where(x => x.CorporationId == user.CorporationId)
            .Include(x => x.Usuario).AsQueryable();

        //if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //{
        //    queryable = queryable.Where(x => x.NroTransfer.Contains(pagination.Filter.ToLower()));
        //}

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.DateTransfer).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transfer>> GetTransfers(int id)
    {
        try
        {
            var modelo = await _context.Transfers.Include(x=> x.Usuario)
            .FirstOrDefaultAsync(x => x.TransferId == id);

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
    public async Task<IActionResult> PutTransfers(Transfer modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();

            _context.Transfers.Update(modelo);
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
    public async Task<ActionResult<Transfer>> PostSells(Transfer modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            var Bodegas = await _context.ProductStorages.Where(x => x.CorporationId == user.CorporationId).ToListAsync();
            if (Bodegas == null)
            {
                return BadRequest("Problemas para Cargar Las Bodegas");
            }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();
            modelo.CorporationId = Convert.ToInt32(user.CorporationId);
            modelo.FromStorageName = Bodegas.Where(x => x.ProductStorageId == modelo.FromProductStorageId).Select(x => x.StorageName).FirstOrDefault();
            modelo.ToStorageName = Bodegas.Where(x => x.ProductStorageId == modelo.ToProductStorageId).Select(x => x.StorageName).FirstOrDefault();
            modelo.Status = TransferType.Pendiente;
            //Para LLevar el control de Consecutivos de Compra
            int ControlTranfer = 0;
            var CheckRegister = await _context.Registers.FirstOrDefaultAsync(x => x.CorporationId == modelo.CorporationId);
            if (CheckRegister == null)
            {
                Register nReg = new()
                {
                    RegPurchase = 0,
                    RegSells = 0,
                    RegTransfer = 1,
                    CorporationId = modelo.CorporationId
                };
                ControlTranfer = 1;
                _context.Registers.Add(nReg);
            }
            else
            {
                CheckRegister.RegTransfer += 1;
                ControlTranfer = CheckRegister.RegTransfer;
                _context.Registers.Update(CheckRegister);
            }
            //Fin...
            modelo.NroTransfer = ControlTranfer;
            _context.Transfers.Add(modelo);
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
    public async Task<IActionResult> DeleteSells(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.Transfers.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.Transfers.Remove(DataRemove);
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