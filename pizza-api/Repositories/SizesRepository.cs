using Dapper;
using Microsoft.Data.Sqlite;

public class SizesRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Size>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);
        return [.. await conn.QueryAsync<Size>("SELECT * FROM sizes")];
    }

    public async Task<Size?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Size>(
            "SELECT * FROM sizes WHERE id = @Id",
            new { Id = id }
        );
    }

    public async Task Create(Size size)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "INSERT INTO sizes (name, diameter, price) VALUES (@Name, @Diameter, @Price)",
            size
        );
    }

    public async Task Update(Size size)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "UPDATE sizes SET name = @Name, diameter = @Diameter, price = @Price WHERE id = @Id",
            size
        );
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var size = await conn.QuerySingleOrDefaultAsync<Size>(
            "SELECT * FROM sizes WHERE id = @Id",
            new { Id = id }
        );

        if (size == null)
            return false;

        await conn.ExecuteAsync("DELETE FROM sizes WHERE id = @Id", new { Id = id });

        return true;
    }
}
