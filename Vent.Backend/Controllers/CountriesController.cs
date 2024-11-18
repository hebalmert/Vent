using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vent.DataAccess;
using Vent.Services.Interfaces;
using Vent.Shared.Entities;

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
    public async Task<ActionResult<IEnumerable<Country>>> Get()
    {
        var CountryList = await _countryService.GetAsync();

        return Ok(CountryList);
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Country>>> GetAll()
    {
        var CountryList = await _countryService.GetAllAsync();

        return Ok(CountryList);
    }
}