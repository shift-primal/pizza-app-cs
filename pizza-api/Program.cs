public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("Default")!;

        builder.Services.AddScoped(_ => new SizesRepository(connectionString));
        builder.Services.AddScoped(_ => new CustomersRepository(connectionString));
        builder.Services.AddScoped(_ => new OrdersRepository(connectionString));
        builder.Services.AddScoped(_ => new ToppingsRepository(connectionString));
        builder.Services.AddScoped(_ => new PizzasRepository(connectionString));
        builder.Services.AddScoped(_ => new DoughsRepository(connectionString));

        builder.Services.AddControllers();

        var app = builder.Build();

        DbInit.Init(connectionString);

        app.MapControllers();

        app.Run();
    }
}
