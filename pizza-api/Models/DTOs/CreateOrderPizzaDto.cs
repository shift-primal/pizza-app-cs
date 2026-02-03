using System.ComponentModel.DataAnnotations;

public record CreateOrderPizzaDto(
    [Required] int SizeId,
    [Required] int DoughId,
    List<int>? ToppingIds
);
