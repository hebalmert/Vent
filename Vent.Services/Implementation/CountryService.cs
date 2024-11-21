using Vent.Repositories.Interfaces;
using Vent.Services.Interfaces;
using Vent.Shared.Entities;
using Vent.Shared.Pagination;
using Vent.Shared.Responses;

namespace Vent.Services.Implementation;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<Response> GetAsync(PaginationDTO pagination) => await _countryRepository.GetAsync(pagination);
}