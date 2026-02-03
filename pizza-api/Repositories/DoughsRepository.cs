using Dapper;
using Microsoft.Data.Sqlite;

public class DoughsRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<List<Dough>> GetAll()
    {
        using var conn = new SqliteConnection(_connectionString);
        return [.. await conn.QueryAsync<Dough>("SELECT * FROM doughs")];
    }

    public async Task<Dough?> GetById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Dough>(
            "SELECT * FROM doughs WHERE id = @Id",
            new { Id = id }
        );
    }

    public async Task Create(Dough dough)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "INSERT INTO doughs (name, price, glutenfree) VALUES (@Name, @Price, @GlutenFree)",
            dough
        );
    }

    public async Task Update(Dough dough)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.ExecuteAsync(
            "UPDATE doughs SET name = @Name, price = @Price, glutenfree = @GlutenFree WHERE id = @Id",
            dough
        );
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = new SqliteConnection(_connectionString);

        var dough = await conn.QuerySingleOrDefaultAsync<Dough>(
            "SELECT * FROM doughs WHERE id = @Id",
            new { Id = id }
        );

        if (dough == null)
            return false;

        await conn.ExecuteAsync("DELETE FROM doughs WHERE id = @Id", new { Id = id });

        return true;
    }
}
