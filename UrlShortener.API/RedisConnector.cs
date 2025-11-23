using StackExchange.Redis;

namespace UrlShortener.API;

public static class RedisConnector
{
    private const string _connectionString = "localhost:6379";
    private static ConnectionMultiplexer? _redis;

    private static void Connect()
    {
        _redis = ConnectionMultiplexer.Connect(_connectionString);
    }

    private static IDatabase GetDatabase()
    {
        if (_redis == null || !_redis.IsConnected)
        {
            throw new InvalidOperationException("Redis connection not established.");
        }
        return _redis.GetDatabase();
    }

    public static long GetNextSeedNumber()
    {
        Connect();
        var database = GetDatabase();
        //var testeset = database.StringSet(key: "url-shortener-number-seed", value: "15000000", flags: CommandFlags.None); //it should be set manually in Redis
        var teste = database.StringIncrement(key: "url-shortener-number-seed", value: 1, flags: CommandFlags.None); //sets the initial value to 15.000.000
        Dispose();
        return teste;
    }

    private static void Dispose()
    {
        _redis?.Dispose();
    }
}