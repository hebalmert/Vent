using Vent.Repositories.Interfaces;
using Vent.Services.Interfaces;
using Vent.Shared.Entities;

namespace Vent.Services.Implementation;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<IEnumerable<Country>> GetAllAsync() => await _countryRepository.GetAllAsync();

    public async Task<IEnumerable<Country>> GetAsync() => await _countryRepository.GetAsync();
}