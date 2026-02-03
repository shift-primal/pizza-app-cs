public class PizzaService
{
    private readonly ApiClient _api = new();

    public async Task<List<Pizza>> GetAll()
    {
        return await _api.Get<List<Pizza>>("/pizzas");
    }

    public async Task<Pizza> GetById(int id)
    {
        return await _api.Get<Pizza>($"/pizzas/{id}");
    }

    public async Task<Pizza> Create(Pizza pizza)
    {
        return await _api.Post<Pizza>("/pizzas", pizza);
    }

    public async Task<Pizza> Update(Pizza pizza)
    {
        return await _api.Put<Pizza>($"/pizzas/{pizza.Id}", pizza);
    }

    public Task Delete(int id)
    {
        return _api.Delete($"/pizzas/{id}");
    }

    // public async Task<List<Pizza>> Search() { }
}
