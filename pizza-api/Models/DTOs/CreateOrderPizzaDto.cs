using System.ComponentModel.DataAnnotations;

public record CreateOrderPizzaDto([Required] int SizeId, List<int>? ToppingIds);
