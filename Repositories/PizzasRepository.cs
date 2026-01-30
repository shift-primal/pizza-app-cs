using Dapper;
using Microsoft.Data.Sqlite;

public class PizzasRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Pizza>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);
        return [.. await conn.QueryAsync<Pizza>("SELECT * FROM pizzas")];
    }

    public async Task<Pizza?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Pizza>(
            "SELECT * FROM pizzas WHERE id = @Id",
            new { Id = id }
        );
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

    public async Task<List<Topping>> GetToppings(int pizzaId)
    {
        using var conn = new SqliteConnection(_connectionString);
        return
        [
            .. await conn.QueryAsync<Topping>(
                "SELECT t.* FROM toppings t INNER JOIN pizzatoppings pt ON t.id = pt.toppingid WHERE pt.pizzaid = @PizzaId",
                new { PizzaId = pizzaId }
            ),
        ];
    }

    public async Task AddTopping(int pizzaId, int toppingId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "INSERT INTO pizzatoppings (pizzaid, toppingid) VALUES (@pizzaId, @toppingId)",
            new { pizzaid = pizzaId, toppingid = toppingId }
        );
    }

    public async Task RemoveTopping(int pizzaId, int toppingId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "DELETE FROM pizzatoppings WHERE pizzaid = @pizzaId AND toppingid = @toppingId",
            new { pizzaid = pizzaId, toppingid = toppingId }
        );
    }
}
