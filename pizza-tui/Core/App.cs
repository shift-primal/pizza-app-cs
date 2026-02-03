public class App()
{
    public async Task Run()
    {
        Console.WriteLine("App starting...");

        PizzaService ps = new();

        List<Pizza> pizzas = await ps.GetAll();
    }
}
