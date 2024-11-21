using Microsoft.AspNetCore.Mvc;
using Vent.Services.Interfaces;
using Vent.Shared.Entities;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> Get([FromQuery] PaginationDTO pagination)
    {
        var response = await _countryService.GetAsync(pagination);
        if (response.IsSuccess)
        {
            var lista = (List<Country>)response.Result!;
            HttpContext.Response.Headers.Append("conteo", response.CountItem.ToString());
            return Ok(lista);
        }

        return BadRequest("Error de Lctura");
    }
}