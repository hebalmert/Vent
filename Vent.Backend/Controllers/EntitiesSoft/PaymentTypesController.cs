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
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers.EntitiesSoft;

[Route("api/paymentTypes")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class PaymentTypesController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public PaymentTypesController(DataContext context, IFileStorage fileStorage,
        IConfiguration configuration, IUserHelper userHelper)
    {
        _context = context;
        _fileStorage = fileStorage;
        _configuration = configuration;
        _userHelper = userHelper;
    }

    [HttpGet("loadCombo")]
    public async Task<ActionResult<IEnumerable<PaymentType>>> GetComboPayments()
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var listResult = await _context.PaymentTypes.Where(x => x.Active && x.CorporationId == user.CorporationId).ToListAsync();
        return listResult;
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentType>>> GetPaymentType([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.PaymentTypes.Where(x => x.CorporationId == user.CorporationId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.PaymentName!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.PaymentName).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentType>> GetPaymentType(int id)
    {
        try
        {
            var modelo = await _context.PaymentTypes
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
    public async Task<IActionResult> PutPaymentType(PaymentType modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();
            _context.PaymentTypes.Update(modelo);
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
    public async Task<ActionResult<PaymentType>> PostPaymentType(PaymentType modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();

            modelo.CorporationId = Convert.ToInt32(user.CorporationId);
            _context.PaymentTypes.Add(modelo);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetPaymentType", new { id = modelo.PaymentTypeId }, modelo);
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
    public async Task<IActionResult> DeleteTaxes(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.PaymentTypes.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.PaymentTypes.Remove(DataRemove);
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