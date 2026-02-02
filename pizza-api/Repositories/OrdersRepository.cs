using Dapper;
using Microsoft.Data.Sqlite;

public class OrdersRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Order>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);

        List<Order> orders = [.. await conn.QueryAsync<Order>("SELECT * FROM orders")];

        await OrderLoader.LoadPizzas(conn, orders);

        return orders;
    }

    public async Task<Order?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var order = await conn.QuerySingleOrDefaultAsync<Order>(
            "SELECT * FROM orders WHERE id = @Id",
            new { Id = id }
        );

        if (order == null)
            return null;

        await OrderLoader.LoadPizzas(conn, order);

        return order;
    }

    public async Task<int> Create(Order order)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO orders (customerid, date, total) VALUES (@CustomerId, @Date, @Total);
            SELECT last_insert_rowid();",
            order
        );
    }

    public async Task Update(Order order)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "UPDATE orders SET customerid = @CustomerId, date = @Date, total = @Total WHERE id = @Id",
            order
        );
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var order = await conn.QuerySingleOrDefaultAsync<Order>(
            "SELECT * FROM orders WHERE id = @Id",
            new { Id = id }
        );

        if (order == null)
            return false;

        var pizzas = await conn.QueryAsync<Pizza>(
            "SELECT * FROM pizzas WHERE orderid = @Id",
            new { Id = id }
        );

        var pizzaIds = pizzas.Select(p => p.Id).ToList();

        await conn.ExecuteAsync(
            @"
              DELETE FROM pizzatoppings WHERE pizzaid IN @PizzaIds;
              DELETE FROM pizzas WHERE orderid = @Id;
              DELETE FROM orders WHERE id = @Id;
                ",
            new { PizzaIds = pizzaIds, Id = id }
        );

        return true;
    }
}
