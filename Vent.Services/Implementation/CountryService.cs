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

    public async Task<Response> GetIdAsync(int Id) => await _countryRepository.GetIdAsync(Id);

    public async Task<Response> PostAsync(Country country) => await _countryRepository.PostAsync(country);

    public async Task<Response> PutAsync(Country country) => await _countryRepository.PutAsync(country);

    public async Task<Response> DeleteAsync(int Id) => await _countryRepository.DeleteAsync(Id);
}