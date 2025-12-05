using FGC.Games.Domain.Entities;
using FGC.Games.Domain.Enums;
using FGC.Games.Domain.Interfaces;
using FGC.Games.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FGC.Games.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GamesDbContext _context;

        public GameRepository(GamesDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Game> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games
                .AsNoTracking()
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetByCategoryAsync(GameCategory category)
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.Category == category && g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetActiveGamesAsync()
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> SearchByTitleAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveGamesAsync();

            return await _context.Games
                .AsNoTracking()
                .Where(g => g.Title.Contains(searchTerm) && g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetTopRatedAsync(int count = 10)
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.IsActive)
                .OrderByDescending(g => g.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetMostSoldAsync(int count = 10)
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.IsActive)
                .OrderByDescending(g => g.TotalSales)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            return await _context.Games
                .AsNoTracking()
                .AnyAsync(g => g.Title.ToLower() == title.ToLower());
        }

        public async Task SaveAsync(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var existingGame = await _context.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == game.Id);

            if (existingGame == null)
            {
                await _context.Games.AddAsync(game);
            }
            else
            {
                _context.Games.Update(game);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                return;

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game != null)
            {
                game.Deactivate();
                await SaveAsync(game);
            }
        }
    }
}
