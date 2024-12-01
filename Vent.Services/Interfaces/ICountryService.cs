using Vent.Shared.Entities;
using Vent.Shared.Pagination;
using Vent.Shared.Responses;

namespace Vent.Services.Interfaces;

public interface ICountryService
{
    Task<Response> GetAsync(PaginationDTO pagination);

    Task<Response> GetIdAsync(int Id);

    Task<Response> PostAsync(Country country);

    Task<Response> PutAsync(Country country);

    Task<Response> DeleteAsync(int Id);
}