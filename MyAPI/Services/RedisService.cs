using StackExchange.Redis;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService(IConfiguration config)
    {
        var raw = config["Redis:Connection"]
            ?? throw new Exception("Missing Redis connection");

        ConfigurationOptions options;

        // 👉 nếu là dạng URI (rediss://)
        if (raw.StartsWith("redis://") || raw.StartsWith("rediss://"))
        {
            var uri = new Uri(raw);

            var userInfo = uri.UserInfo.Split(':');

            options = new ConfigurationOptions
            {
                EndPoints = { { uri.Host, uri.Port } },
                User = userInfo.Length > 1 ? userInfo[0] : null,
                Password = userInfo.Length > 1 ? userInfo[1] : userInfo[0],
                Ssl = raw.StartsWith("rediss://"),
                AbortOnConnectFail = false,
                ConnectRetry = 3,
                ConnectTimeout = 10000,
            };
        }
        else
        {
            // 👉 dạng Render ENV: host:port,password=...
            options = ConfigurationOptions.Parse(raw);
            options.AbortOnConnectFail = false;
        }

        var redis = ConnectionMultiplexer.Connect(options);
        _db = redis.GetDatabase();
    }

    public IDatabase Db => _db;
}