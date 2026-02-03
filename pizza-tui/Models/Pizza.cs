public class Pizza
{
    public int Id { get; set; }
    public Size? Size { get; set; }
    public Dough? Dough { get; set; }
    public List<Topping> Toppings { get; set; } = [];
    public decimal Price { get; set; }
}
