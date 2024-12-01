using Microsoft.AspNetCore.Mvc;
using Vent.Services.Interfaces;
using Vent.Shared.Entities;
using Vent.Shared.Pagination;

namespace Vent.Backend.Controllers;

[Route("api/countries")]
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Country>> GetIdAsuny(int id)
    {
        var response = await _countryService.GetIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response.Result!);
        }
        return BadRequest("NO se Encuentra el Registro");
    }

    [HttpPost]
    public async Task<ActionResult<Country>> GetPostAsync([FromBody] Country country)
    {
        var response = await _countryService.PostAsync(country);
        if (response.IsSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest("Error de Guardado");
    }

    [HttpPut]
    public async Task<ActionResult<Country>> PutAsync([FromBody] Country country)
    {
        var response = await _countryService.PutAsync(country);
        if (response.IsSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest("Error en Actualizacion");
    }
}