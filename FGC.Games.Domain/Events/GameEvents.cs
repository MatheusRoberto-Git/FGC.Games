using FGC.Games.Domain.Common.Events;

namespace FGC.Games.Domain.Events
{
    public class GameCreatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public decimal Price { get; }
        public DateTime CreatedAt { get; }

        public GameCreatedEvent(Guid gameId, string title, decimal price, DateTime createdAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            Price = price;
            CreatedAt = createdAt;
        }
    }

    public class GamePriceUpdatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public DateTime UpdatedAt { get; }

        public GamePriceUpdatedEvent(Guid gameId, string title, decimal oldPrice, decimal newPrice, DateTime updatedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            UpdatedAt = updatedAt;
        }
    }

    public class GameDeactivatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public DateTime DeactivatedAt { get; }

        public GameDeactivatedEvent(Guid gameId, string title, DateTime deactivatedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            DeactivatedAt = deactivatedAt;
        }
    }

    public class GameActivatedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredAt { get; }
        public Guid GameId { get; }
        public string Title { get; }
        public DateTime ActivatedAt { get; }

        public GameActivatedEvent(Guid gameId, string title, DateTime activatedAt)
        {
            Id = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            GameId = gameId;
            Title = title;
            ActivatedAt = activatedAt;
        }
    }
}
