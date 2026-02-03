using Dapper;
using Microsoft.Data.Sqlite;

public static class PizzaLoader
{
    public static async Task LoadDetails(SqliteConnection conn, Pizza pizza)
    {
        await LoadDetails(conn, [pizza]);
    }

    public static async Task LoadDetails(SqliteConnection conn, List<Pizza> pizzas)
    {
        if (pizzas.Count == 0)
            return;

        var pizzaIds = pizzas.Select(p => p.Id).ToList();
        var sizeIds = pizzas.Select(p => p.SizeId).Distinct().ToList();
        var doughIds = pizzas.Select(p => p.DoughId).Distinct().ToList();

        var sizes = await conn.QueryAsync<Size>(
            "SELECT * FROM sizes WHERE id IN @SizeIds",
            new { SizeIds = sizeIds }
        );

        var doughs = await conn.QueryAsync<Dough>(
            "SELECT * FROM doughs WHERE id IN @DoughIds",
            new { DoughIds = doughIds }
        );

        var pizzaToppings = await conn.QueryAsync<PizzaTopping>(
            "SELECT * FROM pizzatoppings WHERE pizzaid IN @PizzaIds",
            new { PizzaIds = pizzaIds }
        );

        var toppingIds = pizzaToppings.Select(pt => pt.ToppingId).Distinct().ToList();

        var toppings =
            toppingIds.Count > 0
                ? await conn.QueryAsync<Topping>(
                    "SELECT * FROM toppings WHERE id IN @ToppingIds",
                    new { ToppingIds = toppingIds }
                )
                : [];

        foreach (var pizza in pizzas)
        {
            pizza.Size = sizes.FirstOrDefault(s => s.Id == pizza.SizeId);
            pizza.Dough = doughs.FirstOrDefault(d => d.Id == pizza.DoughId);

            var toppingIdsForPizza = pizzaToppings
                .Where(pt => pt.PizzaId == pizza.Id)
                .Select(pt => pt.ToppingId);
            pizza.Toppings = [.. toppings.Where(t => toppingIdsForPizza.Contains(t.Id))];
        }
    }
}
