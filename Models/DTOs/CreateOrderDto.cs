using System.ComponentModel.DataAnnotations;

public record CreateOrderDto(
    [Required] int CustomerId,
    [Required] DateTime Date,
    [Required, Range(0.01, double.MaxValue)] decimal Total,
    List<CreateOrderPizzaDto>? Pizzas
);
