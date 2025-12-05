using FGC.Games.Application.DTOs;
using FGC.Games.Domain.Entities;
using FGC.Games.Domain.Interfaces;

namespace FGC.Games.Application.UseCases
{
    public class CreateGameUseCase
    {
        private readonly IGameRepository _gameRepository;

        public CreateGameUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(CreateGameDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (await _gameRepository.ExistsByTitleAsync(dto.Title))
                throw new InvalidOperationException($"Já existe um jogo com o título '{dto.Title}'");

            var game = Game.Create(
                dto.Title,
                dto.Description,
                dto.Price,
                dto.Category,
                dto.Developer,
                dto.Publisher,
                dto.ReleaseDate
            );

            await _gameRepository.SaveAsync(game);
            game.ClearDomainEvents();

            return MapToResponseDto(game);
        }

        private static GameResponseDTO MapToResponseDto(Game game)
        {
            return new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            };
        }
    }

    public class GetGameByIdUseCase
    {
        private readonly IGameRepository _gameRepository;

        public GetGameByIdUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(Guid gameId)
        {
            if (gameId == Guid.Empty)
                throw new ArgumentException("ID do jogo é obrigatório", nameof(gameId));

            var game = await _gameRepository.GetByIdAsync(gameId);

            if (game == null)
                throw new InvalidOperationException($"Jogo com ID {gameId} não encontrado");

            return new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            };
        }
    }

    public class GetAllGamesUseCase
    {
        private readonly IGameRepository _gameRepository;

        public GetAllGamesUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<IEnumerable<GameResponseDTO>> ExecuteAsync(bool onlyActive = true)
        {
            var games = onlyActive
                ? await _gameRepository.GetActiveGamesAsync()
                : await _gameRepository.GetAllAsync();

            return games.Select(game => new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            });
        }
    }

    public class SearchGamesUseCase
    {
        private readonly IGameRepository _gameRepository;

        public SearchGamesUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<IEnumerable<GameResponseDTO>> ExecuteAsync(SearchGamesDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            IEnumerable<Game> games;

            if (!string.IsNullOrWhiteSpace(dto.SearchTerm))
            {
                games = await _gameRepository.SearchByTitleAsync(dto.SearchTerm);
            }
            else if (dto.Category.HasValue)
            {
                games = await _gameRepository.GetByCategoryAsync(dto.Category.Value);
            }
            else
            {
                games = dto.OnlyActive
                    ? await _gameRepository.GetActiveGamesAsync()
                    : await _gameRepository.GetAllAsync();
            }

            if (dto.MinPrice.HasValue)
                games = games.Where(g => g.Price >= dto.MinPrice.Value);

            if (dto.MaxPrice.HasValue)
                games = games.Where(g => g.Price <= dto.MaxPrice.Value);

            if (dto.OnlyActive)
                games = games.Where(g => g.IsActive);

            return games.Select(game => new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            });
        }
    }

    public class UpdateGamePriceUseCase
    {
        private readonly IGameRepository _gameRepository;

        public UpdateGamePriceUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(UpdateGamePriceDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var game = await _gameRepository.GetByIdAsync(dto.GameId);

            if (game == null)
                throw new InvalidOperationException($"Jogo com ID {dto.GameId} não encontrado");

            game.UpdatePrice(dto.NewPrice);

            await _gameRepository.SaveAsync(game);
            game.ClearDomainEvents();

            return new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            };
        }
    }

    public class DeactivateGameUseCase
    {
        private readonly IGameRepository _gameRepository;

        public DeactivateGameUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(Guid gameId)
        {
            if (gameId == Guid.Empty)
                throw new ArgumentException("ID do jogo é obrigatório", nameof(gameId));

            var game = await _gameRepository.GetByIdAsync(gameId);

            if (game == null)
                throw new InvalidOperationException($"Jogo com ID {gameId} não encontrado");

            game.Deactivate();

            await _gameRepository.SaveAsync(game);
            game.ClearDomainEvents();

            return new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            };
        }
    }

    public class ActivateGameUseCase
    {
        private readonly IGameRepository _gameRepository;

        public ActivateGameUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(Guid gameId)
        {
            if (gameId == Guid.Empty)
                throw new ArgumentException("ID do jogo é obrigatório", nameof(gameId));

            var game = await _gameRepository.GetByIdAsync(gameId);

            if (game == null)
                throw new InvalidOperationException($"Jogo com ID {gameId} não encontrado");

            game.Activate();

            await _gameRepository.SaveAsync(game);
            game.ClearDomainEvents();

            return new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            };
        }
    }
}