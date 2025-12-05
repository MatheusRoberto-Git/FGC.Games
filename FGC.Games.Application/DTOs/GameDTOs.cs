using FGC.Games.Domain.Enums;

namespace FGC.Games.Application.DTOs
{
    public class CreateGameDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public GameCategory Category { get; set; }
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
    }

    public class UpdateGameDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
    }

    public class UpdateGamePriceDTO
    {
        public Guid GameId { get; set; }
        public decimal NewPrice { get; set; }
    }

    public class GameResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public double Rating { get; set; }
        public int TotalSales { get; set; }
    }

    public class SearchGamesDTO
    {
        public string SearchTerm { get; set; } = string.Empty;
        public GameCategory? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool OnlyActive { get; set; } = true;
    }
}