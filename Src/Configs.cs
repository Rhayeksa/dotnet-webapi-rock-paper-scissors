namespace dotnet_webapi_rock_paper_scissors.Src.Configs;

using Npgsql;
using DotNetEnv;

public static class Configs
{
    static Configs() { Env.Load(); }

    // PosgreSQL
    public static string PostgresHost => Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "127.0.0.1";
    public static string PostgresPort => Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    public static string PostgresDb => Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "postgres";
    public static string PostgresUsername => Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres";
    public static string PostgresPassword => Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
    public static string PostgresSchema => Environment.GetEnvironmentVariable("POSTGRES_SCHEMA") ?? "public";
    public static NpgsqlConnection GetConnection()
        // => new NpgsqlConnection(AppConfig.PostgresConnectionString);
        => new(
            $"Host={PostgresHost};Port={PostgresPort};Database={PostgresDb};Username={PostgresUsername};Password={PostgresPassword};Pooling=true;Maximum Pool Size=20;"
        );

    // JwtToken
    public static string JwtSecretKey => Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
    public static string JwtAlgorithm => Environment.GetEnvironmentVariable("JWT_ALGORITHM") ?? "HS256";
    public static double JwtAccessTokenExpireHours => double.TryParse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRE_HOURS"), out var hours) ? hours : 1;
}
