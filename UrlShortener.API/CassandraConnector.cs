using Cassandra;

namespace UrlShortener.API;

public static class CassandraConnector
{

    public static async Task Connect()
    {
        try
        {
            // Connect to the Cassandra cluster
            // Use the IP address and port where your Dockerized Cassandra is accessible
            var cluster = Cluster.Builder()
                .AddContactPoint("127.0.0.1") // Or the IP of your Docker host if remote
                .WithPort(9042)
                .Build();

            var session = await cluster.ConnectAsync();

            Console.WriteLine("Connected to Cassandra!");

            // Example: Create a keyspace and table (if they don't exist)
            await session.ExecuteAsync(new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS mykeyspace WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};"));
            await session.ExecuteAsync(new SimpleStatement("USE mykeyspace;"));
            //await session.ExecuteAsync(new SimpleStatement("CREATE TABLE IF NOT EXISTS users (id UUID PRIMARY KEY, name text, age int);"));

            // Example: Insert data
            //var userId = Guid.NewGuid();
            //await session.ExecuteAsync(new SimpleStatement($"INSERT INTO users (id, name, age) VALUES ({userId}, 'John Doe', 30);"));
            //Console.WriteLine("Data inserted.");

            // Example: Query data
            var rowSet = await session.ExecuteAsync(new SimpleStatement("SELECT * FROM users;"));

            if (rowSet.IsFullyFetched)
            {
                var teste = rowSet.GetRows();
                
                foreach (var row in teste)
                {
                    Console.WriteLine($"Id: {row.GetValue<Guid>("id")}, User: {row.GetValue<string>("name")}, Age: {row.GetValue<int>("age")}");
                }
            }

            await session.ShutdownAsync();
            await cluster.ShutdownAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to Cassandra: {ex.Message}");
        }
    }
}