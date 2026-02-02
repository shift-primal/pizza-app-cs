public class Pizza
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int SizeId { get; set; }
    public Size? Size { get; set; }
    public List<Topping> Toppings { get; set; } = [];
}
