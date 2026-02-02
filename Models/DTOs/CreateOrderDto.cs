public record CreateOrderDto(
    int CustomerId,
    DateTime Date,
    decimal Total,
    List<CreateOrderPizzaDto>? Pizzas
);
