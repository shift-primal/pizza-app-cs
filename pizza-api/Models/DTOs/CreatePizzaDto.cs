using System.ComponentModel.DataAnnotations;

public record CreatePizzaDto(
    [Required] int OrderId,
    [Required] int SizeId,
    [Required] int DoughId,
    List<int>? ToppingIds
);
