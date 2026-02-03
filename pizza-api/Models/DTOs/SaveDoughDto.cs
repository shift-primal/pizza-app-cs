using System.ComponentModel.DataAnnotations;

public record CreateDoughDto([Required] string Name, bool GlutenFree, decimal Price);
