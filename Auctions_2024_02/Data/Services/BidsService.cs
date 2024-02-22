using Auctions_2024_02.Models;
using Microsoft.EntityFrameworkCore;

namespace Auctions_2024_02.Data.Services
{
    public class BidsService : IBidsService
    {
        private readonly ApplicationDbContext _context;
        public BidsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Add(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Bid> GetAll()
        {
            var applicationDbContext= from a in _context.Bids.Include(l=>l.Listing).ThenInclude(l=>l.User) select a;
            return applicationDbContext;
        }
    }
}
