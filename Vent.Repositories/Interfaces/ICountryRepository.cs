using Vent.Shared.Entities;
using Vent.Shared.Pagination;
using Vent.Shared.Responses;

namespace Vent.Repositories.Interfaces;

public interface ICountryRepository
{
    Task<Response> GetAsync(PaginationDTO pagination);
}