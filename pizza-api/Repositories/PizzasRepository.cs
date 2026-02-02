using Dapper;
using Microsoft.Data.Sqlite;

public class PizzasRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Pizza>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);

        List<Pizza> pizzas = [.. await conn.QueryAsync<Pizza>("SELECT * FROM pizzas")];

        await PizzaLoader.LoadDetails(conn, pizzas);

        return pizzas;
    }

    public async Task<Pizza?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var pizza = await conn.QuerySingleOrDefaultAsync<Pizza>(
            "SELECT * FROM pizzas WHERE id = @Id",
            new { Id = id }
        );

        if (pizza == null)
            return null;

        await PizzaLoader.LoadDetails(conn, pizza);

        return pizza;
    }

    public async Task<int> Create(Pizza pizza)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO pizzas (orderid, sizeid) VALUES (@OrderId, @SizeId);
            SELECT last_insert_rowid();",
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

    public async Task<bool> Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var pizza = await conn.QuerySingleOrDefaultAsync<Pizza>(
            "SELECT * FROM pizzas WHERE id = @Id",
            new { Id = id }
        );

        if (pizza == null)
            return false;

        await conn.ExecuteAsync(
            @"
            DELETE FROM pizzatoppings WHERE pizzaid = @Id;
            DELETE FROM pizzas WHERE id = @Id",
            new { Id = id }
        );

        return true;
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
