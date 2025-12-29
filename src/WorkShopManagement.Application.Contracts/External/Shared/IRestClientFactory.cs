using RestSharp;

namespace WorkShopManagement.External.Shared;

public interface IRestClientFactory
{
    RestClient Create(string baseUrl, RestClientProfile? profile = null);
}
