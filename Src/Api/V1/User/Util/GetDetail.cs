namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util.GetDetail;

using Npgsql;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Util.JwtToken;
using dotnet_webapi_rock_paper_scissors.Src.Configs;

public static class UtilGetDetail
{
    private static async Task<Dictionary<string, object>?> GetUserAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        string userId
    )
    {
        await using var cmd = new NpgsqlCommand(
            $@"
            SELECT
                user_id,
                created_at,
                updated_at,
                username,
                ""password"",
                ""name"",
                age,
                gender,
                ""role""
            FROM {Configs.PostgresSchema}.tb_users
            WHERE deleted_at IS NULL
            AND user_id = @user_id
            LIMIT 1
            ",
            conn,
            tx
        );

        cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Dictionary<string, object>
        {
            ["user_id"] = reader["user_id"],
            ["created_at"] = reader["created_at"],
            ["updated_at"] = reader["updated_at"],
            ["username"] = reader["username"],
            ["password"] = reader["password"],
            ["name"] = reader["name"],
            ["age"] = reader["age"],
            ["gender"] = reader["gender"],
            ["role"] = reader["role"]
        };
    }

    public static async Task<IResult> MapUtilGetDetail(
        string userId,
        string? xAccessToken
    )
    {
        await using var conn = Configs.GetConnection();
        await conn.OpenAsync();

        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            string? payloadUserId = null;
            string? payloadRole = null;

            // 🔐 1. Verify token (kalau ada)
            if (!string.IsNullOrEmpty(xAccessToken))
            {
                var payload = JwtToken.Verify(xAccessToken);

                if (payload == null)
                    return Results.Unauthorized(
                        // ApiResponse.Response(401, "Your session is invalid. Please log in again.")
                    );

                payloadUserId = payload["user_id"]?.ToString();

                var payloadUser = await GetUserAsync(conn, tx, payloadUserId!);
                if (payloadUser == null)
                    return Results.Unauthorized(
                        // ApiResponse.Response(401, "Invalid session.")
                    );

                payloadRole = payloadUser["role"]?.ToString();

                // 🚫 player tidak boleh lihat user lain
                if (payloadRole == "player" && userId != payloadUserId)
                {
                    return Results.BadRequest(
                        ApiResponse.Response(400, "You don't have permission to view other players' data.")
                    );
                }
            }

            // 🔎 2. Ambil user target
            var targetUser = await GetUserAsync(conn, tx, userId);

            if (targetUser == null)
                return Results.NotFound(
                    ApiResponse.Response(404, "user not found")
                );

            // 🚫 game_master tidak boleh lihat administrator
            if (payloadRole == "game_master" &&
                targetUser["role"]?.ToString() == "administrator")
            {
                return Results.BadRequest(
                    ApiResponse.Response(400, "Game masters are not allowed to view administrators' data.")
                );
            }

            await tx.CommitAsync();

            return Results.Ok(
                ApiResponse.Response(200, data: targetUser)
            );
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"Error (GetDetail): {ex.Message}");

            return Results.InternalServerError(
                ApiResponse.Response(500)
            );
        }
    }
}
