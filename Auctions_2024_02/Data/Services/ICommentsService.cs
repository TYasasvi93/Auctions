using Auctions_2024_02.Models;

namespace Auctions_2024_02.Data.Services
{
    public interface ICommentsService
    {
        Task Add(Comment comment);
    }
}
