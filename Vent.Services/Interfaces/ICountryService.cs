using Vent.Shared.Entities;

namespace Vent.Services.Interfaces;

public interface ICountryService
{
    Task<IEnumerable<Country>> GetAsync();

    Task<IEnumerable<Country>> GetAllAsync();
}