using Dapper;
using Microsoft.Data.Sqlite;

public class ToppingsRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Topping>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);
        return [.. await conn.QueryAsync<Topping>("SELECT * FROM toppings")];
    }

    public async Task<Topping?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Topping>(
            "SELECT * FROM toppings WHERE id = @Id",
            new { Id = id }
        );
    }

    public async Task Create(Topping topping)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "INSERT INTO toppings (name, price) VALUES (@Name, @Price)",
            topping
        );
    }

    public async Task Update(Topping topping)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "UPDATE toppings SET name = @Name, price = @Price WHERE id = @Id",
            topping
        );
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var topping = await conn.QuerySingleOrDefaultAsync<Topping>(
            "SELECT * FROM toppings WHERE id = @Id",
            new { Id = id }
        );

        if (topping == null)
            return false;

        await conn.ExecuteAsync("DELETE FROM toppings WHERE id = @Id", new { Id = id });

        return true;
    }
}
