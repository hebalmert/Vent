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

[Route("api/products")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IUserHelper _userHelper;
    private readonly string ImgRoute;

    public ProductsController(DataContext context, IFileStorage fileStorage,
        IConfiguration configuration, IUserHelper userHelper)
    {
        _context = context;
        _fileStorage = fileStorage;
        _configuration = configuration;
        _userHelper = userHelper;
        ImgRoute = "wwwroot\\Images\\ImgProducts";
    }

    [HttpGet("loadCombo/{Id:int}")] //Id sera CategoryId
    public async Task<ActionResult<IEnumerable<Product>>> GetLoadCombos(int Id)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var listResult = await _context.Products.Where(x => x.Active && x.CorporationId == user.CorporationId && x.CategoryId == Id).ToListAsync();
        return listResult;
    }

    // GET: api/Corporations
    [HttpGet("getproductimagen")]
    public async Task<ActionResult<IEnumerable<ProductImage>>> GetProductPhoto([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.ProductImages.Where(x => x.CorporationId == user.CorporationId && x.ProductId == pagination.Id).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Photo!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] PaginationDTO pagination)
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);

        var queryable = _context.Products.Where(x => x.CorporationId == user.CorporationId).Include(x => x.ProductStocks)
            .Include(x => x.Tax).Include(x => x.ProductImages).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.ProductName!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.ProductName).Paginate(pagination).ToListAsync();
    }

    // GET: api/Corporations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProducts(int id)
    {
        try
        {
            var modelo = await _context.Products.Include(x => x.Tax).Include(x => x.ProductStocks)
            .FirstOrDefaultAsync(x => x.ProductId == id);

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
    public async Task<IActionResult> PutProducts(Product modelo)
    {
        try
        {
            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();

            Product Nmodelo = new Product
            {
                ProductId = modelo.ProductId,
                ProductName = modelo.ProductName,
                CategoryId = modelo.CategoryId,
                Description = modelo.Description,
                Costo = modelo.Costo,
                TaxId = modelo.TaxId,
                Price = modelo.Price,
                WithSerials = modelo.WithSerials,
                Active = modelo.Active,
                CorporationId = modelo.CorporationId,
            };

            _context.Products.Update(Nmodelo);
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

    [HttpPost("postProductimage")]
    public async Task<ActionResult<Category>> PostpostProductimage(ProductImage modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();

            modelo.CorporationId = Convert.ToInt32(user.CorporationId);
            if (modelo.ImgBase64 is not null)
            {
                string guid = Guid.NewGuid().ToString() + ".jpg";
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.Photo = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.ProductImages.Add(modelo);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetProductPhoto", new { Id = modelo.ProductId }, modelo);
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
    public async Task<ActionResult<Product>> PostProducts(Product modelo)
    {
        try
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            User user = await _userHelper.GetUserAsync(email);
            if (user == null) { return BadRequest("Problemas de Seguridad en Acceso a Datos"); }

            //En Caso de un fallo regresamos todo en la base de datos
            var transaction = await _context.Database.BeginTransactionAsync();

            modelo.CorporationId = Convert.ToInt32(user.CorporationId);
            _context.Products.Add(modelo);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetProducts", new { id = modelo.CorporationId }, modelo);
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

    [HttpDelete("deleteproductImage/{id:int}")]
    public async Task<IActionResult> DeleteProductImagen(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.ProductImages.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.ProductImages.Remove(DataRemove);
            await _context.SaveChangesAsync();

            if (DataRemove.Photo is not null)
            {
                var response = _fileStorage.DeleteImage(ImgRoute, DataRemove.Photo);
                if (!response)
                {
                    return BadRequest("Problemas para Eliminar el Imagen...");
                }
            }
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducts(int id)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            var DataRemove = await _context.Products.FindAsync(id);
            if (DataRemove == null)
            {
                return NotFound();
            }
            _context.Products.Remove(DataRemove);
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