using Dapper;
using Microsoft.Data.Sqlite;

public static class OrderLoader
{
    public static async Task LoadPizzas(SqliteConnection conn, Order order)
    {
        await LoadPizzas(conn, [order]);
    }

    public static async Task LoadPizzas(SqliteConnection conn, List<Order> orders)
    {
        if (orders.Count == 0)
            return;

        var orderIds = orders.Select(o => o.Id).ToList();

        List<Pizza> pizzas =
        [
            .. await conn.QueryAsync<Pizza>(
                "SELECT * FROM pizzas WHERE orderid IN @OrderIds",
                new { OrderIds = orderIds }
            ),
        ];

        await PizzaLoader.LoadDetails(conn, pizzas);

        foreach (var order in orders)
            order.Pizzas = [.. pizzas.Where(p => p.OrderId == order.Id)];
    }
}
