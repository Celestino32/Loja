namespace LojaVirtual.Application.Reviews;

public record ReviewDto(Guid Id, string CustomerName, int Rating, string? Comment, DateTime CreatedAtUtc);

public record ProductReviewsDto(double AverageRating, int TotalCount, List<ReviewDto> Items);
