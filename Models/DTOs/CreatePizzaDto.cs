using System.ComponentModel.DataAnnotations;

public record CreatePizzaDto([Required] int OrderId, [Required] int SizeId, List<int>? ToppingIds);
