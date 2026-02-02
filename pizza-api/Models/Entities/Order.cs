public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public List<Pizza> Pizzas { get; set; } = [];
}
