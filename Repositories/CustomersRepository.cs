using Dapper;
using Microsoft.Data.Sqlite;

public class CustomersRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Customer>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);

        List<Customer> customers = [.. await conn.QueryAsync<Customer>("SELECT * FROM customers")];

        if (customers.Count == 0)
            return customers;

        var customerIds = customers.Select(c => c.Id).ToList();

        List<Order> orders =
        [
            .. await conn.QueryAsync<Order>(
                "SELECT * FROM orders WHERE customerid IN @CustomerIds",
                new { CustomerIds = customerIds }
            ),
        ];

        await OrderLoader.LoadPizzas(conn, orders);

        foreach (var customer in customers)
            customer.Orders = [.. orders.Where(o => o.CustomerId == customer.Id)];

        return customers;
    }

    public async Task<Customer?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var customer = await conn.QuerySingleOrDefaultAsync<Customer>(
            "SELECT * FROM customers WHERE id = @Id",
            new { Id = id }
        );

        if (customer == null)
            return null;

        List<Order> orders =
        [
            .. await conn.QueryAsync<Order>(
                "SELECT * FROM orders WHERE customerid = @CustomerId",
                new { CustomerId = customer.Id }
            ),
        ];

        await OrderLoader.LoadPizzas(conn, orders);
        customer.Orders = orders;

        return customer;
    }

    public async Task<int> Create(Customer customer)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO customers (name, email, phone) VALUES (@Name, @Email, @Phone);
            SELECT last_insert_rowid();",
            customer
        );
    }

    public async Task Update(Customer customer)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "UPDATE customers SET name = @Name, email = @Email, phone = @Phone WHERE id = @Id",
            customer
        );
    }

    public async Task Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var customer = await conn.QuerySingleOrDefaultAsync<Customer>(
            "SELECT * FROM customers WHERE id = @Id",
            new { Id = id }
        );

        if (customer != null)
        {
            var orders = await conn.QueryAsync<Order>(
                "SELECT * FROM orders WHERE customerid = @CustomerId",
                new { CustomerId = customer.Id }
            );

            var orderIds = orders.Select(o => o.Id).ToList();

            var pizzas = await conn.QueryAsync<Pizza>(
                "SELECT * FROM pizzas WHERE orderid IN @OrderIds",
                new { OrderIds = orderIds }
            );

            var pizzaIds = pizzas.Select(p => p.Id).ToList();

            await conn.ExecuteAsync(
                @"
                DELETE FROM pizzatoppings WHERE pizzaid IN @PizzaIds;
                DELETE FROM pizzas WHERE orderid IN @OrderIds;
                DELETE FROM orders WHERE customerid = @CustomerId;
                DELETE FROM customers WHERE Id = @CustomerId;
                ",
                new
                {
                    PizzaIds = pizzaIds,
                    OrderIds = orderIds,
                    CustomerId = customer.Id,
                }
            );
        }
    }
}
