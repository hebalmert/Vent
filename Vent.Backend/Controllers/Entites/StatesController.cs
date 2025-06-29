using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vent.AccessData.Data;
using Vent.Helpers;
using Vent.Shared.Entities;

namespace Vent.Backend.Controllers.Entites;

[Route("api/states")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class StatesController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;

    public StatesController(DataContext context, IUserHelper userHelper)
    {
        _context = context;
        _userHelper = userHelper;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    [HttpGet("loadComboStates")]
    public async Task<ActionResult<IEnumerable<State>>> GetComboState()
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        User user = await _userHelper.GetUserAsync(email);
        var compania = await _context.Corporations.FirstOrDefaultAsync(x => x.CorporationId == user.CorporationId);

        var listResult = await _context.States.Where(x => x.CountryId == compania!.CountryId).OrderBy(x => x.Name).ToListAsync();
        return listResult;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    [HttpGet]
    public async Task<ActionResult> GetState()
    {
        var listResult = await _context.States.Include(x => x.Cities).ToListAsync();
        return Ok(listResult);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    [HttpGet("{id}")]
    public async Task<ActionResult<State>> GetState(int id)
    {
        try
        {
            var modelo = await _context.States
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