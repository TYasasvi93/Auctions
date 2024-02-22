using Auctions_2024_02.Models;

namespace Auctions_2024_02.Data.Services
{
    public interface IBidsService
    {
        Task Add(Bid bid);
        IQueryable<Bid> GetAll();
    }
}
