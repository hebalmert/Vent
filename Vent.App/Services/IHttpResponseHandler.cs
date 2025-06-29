using Vent.AccessService.Repositories;

namespace Vent.App.Services;

public interface IHttpResponseHandler
{
    Task<bool> HandleErrorAsync<T>(HttpResponseWrapper<T> responseHttp);
}