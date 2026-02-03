using Dapper;
using Microsoft.Data.Sqlite;

public static class DbInit
{
    public static void Init(string connectionString)
    {
        //language=sql
        var sqlInit =
            @"
        CREATE TABLE IF NOT EXISTS customers (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            email TEXT NOT NULL,
            phone TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS doughs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            glutenfree INTEGER NOT NULL DEFAULT 0 CHECK (glutenfree IN (0, 1)),
            price REAL NOT NULL
        );

        CREATE TABLE IF NOT EXISTS sizes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            diameter INTEGER NOT NULL,
            price REAL NOT NULL
        );

        CREATE TABLE IF NOT EXISTS toppings (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            price REAL NOT NULL
        );

        CREATE TABLE IF NOT EXISTS orders (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            customerid INTEGER NOT NULL,
            date TEXT NOT NULL,
            total REAL NOT NULL,
            FOREIGN KEY (customerid) REFERENCES customers (id)
        );

        CREATE TABLE IF NOT EXISTS pizzas (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            orderid INTEGER NOT NULL,
            sizeid INTEGER NOT NULL,
            doughid INTEGER NOT NULL,
            FOREIGN KEY (orderid) REFERENCES orders (id),
            FOREIGN KEY (sizeid) REFERENCES sizes (id),
            FOREIGN KEY (doughid) REFERENCES doughs (id)
        );


        CREATE TABLE IF NOT EXISTS pizzatoppings (
            pizzaid INTEGER NOT NULL,
            toppingid INTEGER NOT NULL,
            PRIMARY KEY (pizzaid, toppingid),
            FOREIGN KEY (pizzaid) REFERENCES pizzas (id),
            FOREIGN KEY (toppingid) REFERENCES toppings (id)
        );
        ";

        //language=sql
        var sqlSeedData =
            @"
        INSERT OR IGNORE INTO sizes (id, name, diameter, price) VALUES
        (1, 'Liten', 24, 80.00),
        (2, 'Medium', 33, 120.00),
        (3, 'Stor', 40, 180.00);

        INSERT OR IGNORE INTO doughs (id, name, glutenfree, price) VALUES
        (1, 'Klassisk', 0, 0.00),
        (2, 'Italiensk', 0, 10.00),
        (3, 'Amerikansk', 0, 15.00),
        (4, 'Fullkorn', 0, 20.00),
        (5, 'Blomkål', 1, 35.00);

        INSERT OR IGNORE INTO toppings (id, name, price) VALUES
        (1, 'Pepperoni', 20.00),
        (2, 'Skinke', 20.00),
        (3, 'Spekeskinke', 30.00),
        (4, 'Sopp', 10.00),
        (5, 'Mais', 10.00),
        (6, 'Paprika', 10.00),
        (7, 'Kaviar', 100.00),
        (8, 'Safran', 500.00);

        INSERT OR IGNORE INTO customers (id, name, email, phone) VALUES
        (1, 'John Doe', 'john@email.com', '555-1234'),
        (2, 'Jane Smith', 'jane@email.com', '555-5678'),
        (3, 'Bob Wilson', 'bob@email.com', '555-9999');

        INSERT OR IGNORE INTO orders (id, customerid, date, total) VALUES
        (1, 1, '2025-01-15', 250.00),
        (2, 2, '2025-01-20', 310.00),
        (3, 1, '2025-01-28', 150.00);

        INSERT OR IGNORE INTO pizzas (id, orderid, sizeid, doughid) VALUES
        (1, 1, 2, 1),
        (2, 1, 1, 2),
        (3, 2, 3, 3),
        (4, 3, 2, 5);

        INSERT OR IGNORE INTO pizzatoppings (pizzaid, toppingid) VALUES
        (1, 1),
        (1, 4),
        (2, 2),
        (3, 1),
        (3, 2),
        (3, 5),
        (4, 1);
        ";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        connection.ExecuteAsync(sqlInit);
        connection.ExecuteAsync(sqlSeedData);
    }
}
