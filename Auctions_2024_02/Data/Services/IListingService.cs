using Auctions_2024_02.Models;

namespace Auctions_2024_02.Data.Services
{
    public interface IListingService
    {
        IQueryable<Listing> GetAllListing();
        Task Add(Listing listing);  
        Task <Listing> GetById(int? id);
        Task SaveChanges();
    }
}
