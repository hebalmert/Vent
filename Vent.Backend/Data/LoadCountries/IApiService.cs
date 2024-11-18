using Vent.Shared.Responses;

namespace Vent.Backend.Data.LoadCountries;

public interface IApiService
{
    Task<Response> GetListAsync<T>(string servicePrefix, string controller);
}