using Vent.Shared.Entities;

namespace Vent.Repositories.Interfaces;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAsync();

    Task<IEnumerable<Country>> GetAllAsync();
}