using Vent.Shared.Entities;
using Vent.Shared.Pagination;
using Vent.Shared.Responses;

namespace Vent.Services.Interfaces;

public interface ICountryService
{
    Task<Response> GetAsync(PaginationDTO pagination);
}