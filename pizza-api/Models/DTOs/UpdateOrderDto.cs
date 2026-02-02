using System.ComponentModel.DataAnnotations;

public record UpdateOrderDto(
    [Required] int CustomerId,
    [Required] DateTime Date,
    [Required, Range(0.01, double.MaxValue)] decimal Total
);
