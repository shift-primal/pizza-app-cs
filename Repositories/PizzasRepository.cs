using Dapper;
using Microsoft.Data.Sqlite;

public class PizzasRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Pizza>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);

        List<Pizza> pizzas = [.. await conn.QueryAsync<Pizza>("SELECT * FROM pizzas")];

        if (pizzas.Count == 0)
            return pizzas;

        var sizeIds = pizzas.Select(p => p.SizeId).Distinct().ToList();

        var sizes = await conn.QueryAsync<Size>(
            "SELECT * FROM sizes WHERE id IN @SizeIds",
            new { SizeIds = sizeIds }
        );

        var pizzaIds = pizzas.Select(p => p.Id).ToList();

        var pizzaToppings = await conn.QueryAsync<PizzaTopping>(
            "SELECT * FROM pizzatoppings WHERE pizzaid IN @PizzaIds",
            new { PizzaIds = pizzaIds }
        );

        var toppingIds = pizzaToppings.Select(pt => pt.ToppingId).Distinct().ToList();

        var toppings = await conn.QueryAsync<Topping>(
            "SELECT * FROM toppings WHERE id IN @ToppingIds",
            new { ToppingIds = toppingIds }
        );

        foreach (var pizza in pizzas)
        {
            pizza.Size = sizes.FirstOrDefault(s => s.Id == pizza.SizeId);

            var toppingIdsForPizza = pizzaToppings
                .Where(pt => pt.PizzaId == pizza.Id)
                .Select(pt => pt.ToppingId);

            pizza.Toppings = [.. toppings.Where(t => toppingIdsForPizza.Contains(t.Id))];
        }

        return pizzas;
    }

    public async Task<Pizza?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var pizza = await conn.QuerySingleOrDefaultAsync<Pizza>(
            "SELECT * FROM pizzas WHERE id = @Id",
            new { Id = id }
        );

        if (pizza != null)
        {
            var size = await conn.QuerySingleOrDefaultAsync<Size>(
                "SELECT * FROM sizes WHERE id = @SizeId",
                new { pizza.SizeId }
            );
            pizza.Size = size;

            List<Topping> toppings =
            [
                .. await conn.QueryAsync<Topping>(
                    @"SELECT t.* FROM toppings t
            INNER JOIN pizzatoppings pt ON t.id = pt.toppingId
            WHERE pt.pizzaid = @Id",
                    new { Id = id }
                ),
            ];
            pizza.Toppings = toppings;

            return pizza;
        }

        return null;
    }

    public async Task Create(Pizza pizza)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            @"INSERT INTO pizzas (orderid, sizeid) VALUES (@OrderId, @SizeId)",
            pizza
        );
    }

    public async Task Update(Pizza pizza)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "UPDATE pizzas SET orderid = @OrderId, sizeid = @SizeId WHERE id = @Id",
            pizza
        );
    }

    public async Task Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync("DELETE FROM pizzas WHERE id = @Id", new { Id = id });
    }

    // Toppings

    public async Task AddTopping(int pizzaId, int toppingId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "INSERT INTO pizzatoppings (pizzaid, toppingid) VALUES (@PizzaId, @ToppingId)",
            new { PizzaId = pizzaId, ToppingId = toppingId }
        );
    }

    public async Task RemoveTopping(int pizzaId, int toppingId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "DELETE FROM pizzatoppings WHERE pizzaid = @PizzaId AND toppingid = @ToppingId",
            new { PizzaId = pizzaId, ToppingId = toppingId }
        );
    }
}
