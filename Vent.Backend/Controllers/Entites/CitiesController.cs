using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vent.AccessData.Data;
using Vent.Shared.Entities;

namespace Vent.Backend.Controllers.Entites;

[Route("api/cities")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly DataContext _context;

    public CitiesController(DataContext context)
    {
        _context = context;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    [HttpGet("loadComboCities/{Id:int}")]
    public async Task<ActionResult<IEnumerable<City>>> GetPeriodicidads(int Id)
    {
        var listResult = await _context.Cities.Where(x => x.StateId == Id).OrderBy(x => x.Name).ToListAsync();
        return listResult;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult> GetCountries()
    {
        var listResult = await _context.Cities.ToListAsync();
        return Ok(listResult);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<City>> GetCorporation(int id)
    {
        try
        {
            var modelo = await _context.Cities
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
}