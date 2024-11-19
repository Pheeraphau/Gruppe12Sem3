using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data;

namespace WebApplication1.Services;

public interface IGeoChangeService
{
    Task<IEnumerable<GeoChange>> GetAllGeoChangesAsync();
    Task<GeoChange> GetGeoChangeByIdAsync(int id);
    Task AddGeoChangeAsync(GeoChange geoChange);
    Task UpdateGeoChangeAsync(GeoChange geoChange);
    Task DeleteGeoChangeAsync(int id);
}
