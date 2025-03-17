using NZWalk.Api.Models.Domain;

namespace NZWalk.Api.Repositories
{
    public interface IRegionRepository
    {
      Task<List<Region>> GetAllAsync();//method of asyncronus function for getting all regions

        Task<Region?> GetByIdAsync(Guid id);//method or defination of asyncronus function for getting region by id

        Task<Region> CreateAsync(Region region);

        Task<Region?> UpdateAsync(Guid id, Region region);

        Task<Region?> DeleteAsync(Guid id);
        
    }
}
