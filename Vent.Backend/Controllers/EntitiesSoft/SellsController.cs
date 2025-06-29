using ClosedXML.Excel;
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

[Route("api/sells")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class SellsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;

    public SellsController(DataContext context, IFileStorage fileStorage,
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
        List<EnumItemModel> list = Enum.GetValues(typeof(SellType)).Cast<SellType>().Select(c => new EnumItemModel()
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
    [HttpGet("ReporteSellDates")]
    public async Task<ActionResult<IEnumerable<Sell>>> GetReporteSellDates([FromQuery] ReportDataDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        DateTime dateInicio = Convert.ToDateTime(pagination.DateStart);
        DateTime dateFin = Convert.ToDateTime(pagination.DateEnd);

        var queryable = await _context.Sells
            .Where(x => x.CorporationId == user.CorporationId && x.SellDate >= dateInicio && x.SellDate <= dateFin)
            .Include(x => x.Client).Include(x => x.Usuario).Include(x => x.ProductStorage).Include(x => x.SellDetails).ToListAsync();

        if (pagination.Id != 0)
        {
            queryable = queryable.Where(x => x.PaymentTypeId == pagination.Id).ToList();
        }
        return queryable.OrderBy(x => x.SellDate).ToList();
    }

    [HttpGet("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] ReportDataDTO modelo)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        DateTime dateInicio = Convert.ToDateTime(modelo.DateStart);
        DateTime dateFin = Convert.ToDateTime(modelo.DateEnd);

        var sells = await _context.Sells
            .Where(x => x.CorporationId == user.CorporationId && x.SellDate >= dateInicio && x.SellDate <= dateFin)
            .Include(x => x.Client).Include(x => x.Usuario).Include(x => x.ProductStorage).Include(x => x.SellDetails).ToListAsync();

        if (modelo.Id != 0)
        {
            sells = sells.Where(x => x.PaymentTypeId == modelo.Id).ToList();
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ventas");

        // Encabezados
        worksheet.Cell(1, 1).Value = "Cliente";
        worksheet.Cell(1, 2).Value = "Venta";
        worksheet.Cell(1, 3).Value = "Fecha";
        worksheet.Cell(1, 4).Value = "SubTotal";
        worksheet.Cell(1, 5).Value = "Impuesto";
        worksheet.Cell(1, 6).Value = "Total";

        // Datos
        int row = 2;
        foreach (var sell in sells)
        {
            worksheet.Cell(row, 1).Value = sell.Client!.FullName;
            worksheet.Cell(row, 2).Value = sell.NroSell;
            worksheet.Cell(row, 3).Value = sell.SellDate.ToString("dd-MM-yyyy");
            worksheet.Cell(row, 4).Value = sell.SubTotalCompra;
            worksheet.Cell(row, 5).Value = sell.ImpuestoTotalCompra;
            worksheet.Cell(row, 6).Value = sell.TotalCompra;
            row++;
        }

        // Formato
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteVentas.xlsx");
    }

    // GET: api/Corporations
    [HttpGet("storageDispatch")]
    public async Task<ActionResult<IEnumerable<Sell>>> GetstorageDispatch([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.Sells.Where(x => x.CorporationId == user.CorporationId && x.Status == SellType.Procesando)
            .Include(x => x.Client).Include(x => x.Usuario).Include(x => x.ProductStorage).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Client!.FullName!.Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.SellDate).ThenBy(x => x.Client!.FullName).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sell>>> GetSells([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.Sells.Where(x => x.CorporationId == user.CorporationId)
            .Include(x => x.Client).Include(x => x.Usuario).Include(x => x.ProductStorage).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Client!.FullName!.Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.Client!.FullName).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Sell>> GetSells(int id)
    {
        try
        {
            var modelo = await _context.Sells.Include(x => x.ProductStorage).Include(x => x.SellDetails)
            .Include(x => x.Client).Include(x => x.Usuario).Include(x => x.PaymentType)
            .FirstOrDefaultAsync(x => x.SellId == id);

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
    public async Task<IActionResult> PutSells(Sell modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();

            Sell NSell = new()
            {
                SellId = modelo.SellId,
                SellDate = modelo.SellDate,
                NroSell = modelo.NroSell,
                UsuarioId = modelo.UsuarioId,
                ClientId = modelo.UsuarioId,
                PaymentTypeId = modelo.PaymentTypeId,
                ProductStorageId = modelo.ProductStorageId,
                Status = modelo.Status,
                CorporationId = modelo.CorporationId
            };

            _context.Sells.Update(NSell);
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
    public async Task<ActionResult<Sell>> PostSells(Sell modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();
            modelo.CorporationId = Convert.ToInt32(user.CorporationId);
            modelo.Status = SellType.Pendiente;
            //Para LLevar el control de Consecutivos de Compra
            int ControlVenta = 0;
            var CheckRegister = await _context.Registers.FirstOrDefaultAsync(x => x.CorporationId == modelo.CorporationId);
            if (CheckRegister == null)
            {
                Register nReg = new()
                {
                    RegPurchase = 0,
                    RegSells = 1,
                    CorporationId = modelo.CorporationId
                };
                ControlVenta = 1;
                _context.Registers.Add(nReg);
            }
            else
            {
                CheckRegister.RegSells += 1;
                ControlVenta = CheckRegister.RegSells;
                _context.Registers.Update(CheckRegister);
            }
            //Fin...
            modelo.NroSell = ControlVenta;
            _context.Sells.Add(modelo);
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

            var DataRemove = await _context.Sells.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.Sells.Remove(DataRemove);
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

    // DELETE: api/Corporations/5
    [HttpDelete("selldispatch/{id}")]
    public async Task<IActionResult> DespacharAsync(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataUpdate = await _context.Sells.FindAsync(id);
            if (DataUpdate == null)
            {
                return NotFound();
            }
            DataUpdate.Status = SellType.Despachado;
            _context.Sells.Update(DataUpdate);
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