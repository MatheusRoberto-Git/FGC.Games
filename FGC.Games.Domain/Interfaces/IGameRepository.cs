using FGC.Games.Domain.Entities;
using FGC.Games.Domain.Enums;

namespace FGC.Games.Domain.Interfaces
{
    public interface IGameRepository
    {
        Task<Game> GetByIdAsync(Guid id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<IEnumerable<Game>> GetByCategoryAsync(GameCategory category);
        Task<IEnumerable<Game>> GetActiveGamesAsync();
        Task<IEnumerable<Game>> SearchByTitleAsync(string searchTerm);
        Task<IEnumerable<Game>> GetTopRatedAsync(int count = 10);
        Task<IEnumerable<Game>> GetMostSoldAsync(int count = 10);
        Task<bool> ExistsByTitleAsync(string title);
        Task SaveAsync(Game game);
        Task DeleteAsync(Guid id);
    }
}
