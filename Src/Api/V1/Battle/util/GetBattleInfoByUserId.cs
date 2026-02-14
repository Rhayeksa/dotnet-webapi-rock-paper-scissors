namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Util.GetBattleInfoByUserId;

using Npgsql;
using dotnet_webapi_rock_paper_scissors.Src.Configs;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Util.JwtToken;
// using dotnet_webapi_rock_paper_scissors.Src.Util.Pagination;

public static class UtilGetBattleInfoByUserId
{
    public static async Task<IResult> MapUtilGetBattleInfoByUserId(
        string userId,
        // int? pageNum,
        // int? pageSize,
        string? xAccessToken
    )
    {
        int pageSize = 10;
        int pageNum = 1;

        // if (pageNum < 1 || pageSize < 1)
        // {
        //     return Results.BadRequest(
        //         ApiResponse.Response(400, "pageNum and pageSize must be greater than 0")
        //     );
        // }

        // ===============================
        // 🔐 VERIFY TOKEN
        // ===============================
        var payload = JwtToken.Verify(xAccessToken ?? "");

        if (payload == null)
        {
            return Results.Unauthorized(
            // ApiResponse.Response(401, "Your session is invalid. Please log in again.")
            );
        }

        var role = payload.ContainsKey("role")
            ? payload["role"]?.ToString()
            : null;

        bool ignoreAdmin = role == "game_master";

        try
        {
            await using var conn = Configs.GetConnection();
            await conn.OpenAsync();

            // ===============================
            // 🏆 HIGHEST WIN STREAK
            // ===============================
            int highestWinStreak = 0;

            await using (var cmd = new NpgsqlCommand(
                $@"
                SELECT MAX(btl.win_streak)
                FROM {Configs.PostgresSchema}.tb_battles btl
                INNER JOIN {Configs.PostgresSchema}.tb_users usr 
                    ON usr.user_id = btl.user_id
                WHERE btl.deleted_at IS NULL
                AND btl.user_id = @user_id
                AND (@ignore_admin = FALSE OR usr.role <> 'administrator')
                ",
                conn))
            {
                cmd.Parameters.AddWithValue("user_id", userId);
                cmd.Parameters.AddWithValue("ignore_admin", ignoreAdmin);

                var result = await cmd.ExecuteScalarAsync();

                if (result != DBNull.Value && result != null)
                    highestWinStreak = Convert.ToInt32(result);
            }

            // ===============================
            // 📜 BATTLE LIST (Pagination)
            // ===============================
            var battles = new List<object>();

            await using (var cmd = new NpgsqlCommand(
                $@"
                SELECT
                    btl.choice_user,
                    btl.choice_computer,
                    btl.result,
                    btl.win_streak,
                    TO_CHAR(btl.created_at, 'YYYY-MM-DD HH24:MI:SS') AS created_at
                FROM {Configs.PostgresSchema}.tb_battles btl
                INNER JOIN {Configs.PostgresSchema}.tb_users usr 
                    ON usr.user_id = btl.user_id
                WHERE btl.deleted_at IS NULL
                AND btl.user_id = @user_id
                AND (@ignore_admin = FALSE OR usr.role <> 'administrator')
                ORDER BY btl.created_at DESC
                LIMIT @limit
                OFFSET @offset
                ",
                conn))
            {
                cmd.Parameters.AddWithValue("user_id", userId);
                cmd.Parameters.AddWithValue("ignore_admin", ignoreAdmin);
                cmd.Parameters.AddWithValue("limit", pageSize);
                cmd.Parameters.AddWithValue("offset", (pageNum - 1) * pageSize);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    battles.Add(new
                    {
                        choice_user = reader.GetString(0),
                        choice_computer = reader.GetString(1),
                        result = reader.GetString(2),
                        win_streak = reader.GetInt32(3),
                        created_at = reader.GetString(4)
                    });
                }
            }

            var data = new
            {
                highest_win_streak = highestWinStreak,
                battles
            };

            return Results.Ok(
                ApiResponse.Response(
                    200,
                    data: data
                    // page: Pagination.Create(
                    //     pageSize: pageSize,
                    //     currentPage: pageNum
                    // )
                )
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}\n");

            return Results.InternalServerError(
                ApiResponse.Response(500)
            );
        }
    }
}
