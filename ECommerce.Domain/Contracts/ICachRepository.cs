

namespace ECommerce.Domain.Contracts
{
    public interface ICachRepository
    {
        Task<string?> GetAsync(string Cachkey);
        Task SetAsync(string Cachkey, string CachValue, TimeSpan timetolive);

    }
}
