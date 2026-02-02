using System.ComponentModel.DataAnnotations;

public record UpdatePizzaDto([Required] int OrderId, [Required] int SizeId);
