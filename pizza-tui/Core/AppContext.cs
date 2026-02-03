public class AppContext
{
    public string? Customer { get; set; }
    public List<Pizza> Cart { get; set; } = [];
    public Pizza? CurrentPizza { get; set; }
}
