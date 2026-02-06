using Npgsql;
using dotnet_webapi_rock_paper_scissors.src.Configs;
using dotnet_webapi_rock_paper_scissors.Src.Util;

namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util;

public static class UtilLogin
{
    public static async Task<IResult> MapUtilLogin(string username, string password)
    {
        await using var conn = Configs.GetConnection();
        await conn.OpenAsync();
        NpgsqlTransaction? tx = null;
        try
        {
            tx = await conn.BeginTransactionAsync();

            string? userId;
            string? hashedPassword;

            await using (var cmd = new NpgsqlCommand(
                $@"
                SELECT user_id, username, ""password""
                FROM {Configs.PostgresSchema}.tb_users
                WHERE deleted_at IS NULL
                AND username = @username
                LIMIT 1
                "
                , conn, tx))
            {
                cmd.Parameters.AddWithValue("username", username);
                await using var reader = cmd.ExecuteReader();
                if (!await reader.ReadAsync())
                    return Results.BadRequest(ApiResponse.Response(400, "Invalid username or password"));
                userId = reader["user_id"]?.ToString();
                hashedPassword = reader["password"]?.ToString();
            }

            if (hashedPassword == null || !Password.Verify(password, hashedPassword))
                return Results.BadRequest(ApiResponse.Response(code: 400, msg: "Invalid username or password"));

            var token = JwtToken.Generate(new Dictionary<string, object>
            {
                ["user_id"] = userId!
            });

            await tx.CommitAsync();
            return Results.Ok(
                ApiResponse.Response(code: 200, data: token)
            );
        }
        catch (Exception ex)
        {
            if (tx != null) await tx.RollbackAsync();
            Console.WriteLine($"Error (login): {ex.Message}");
            return Results.InternalServerError(
                ApiResponse.Response(500)
            );
        }
        finally
        {
            await conn.CloseAsync();
        }
    }
}
