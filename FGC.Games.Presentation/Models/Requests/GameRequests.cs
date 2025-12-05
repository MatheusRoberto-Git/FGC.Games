namespace FGC.Games.Presentation.Models.Requests
{
    public class CreateGameRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Category { get; set; }
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
    }

    public class UpdateGamePriceRequest
    {
        public decimal NewPrice { get; set; }
    }

    public class SearchGamesRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
