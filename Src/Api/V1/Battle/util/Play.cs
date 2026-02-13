namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Util.Play;

using Npgsql;
using dotnet_webapi_rock_paper_scissors.Src.Configs;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Util.JwtToken;
using dotnet_webapi_rock_paper_scissors.Src.Util.TimezoneNow;

public static class UtilPlay
{
    private static readonly string[] OPTIONS = { "rock", "paper", "scissors" };

    private static readonly Dictionary<string, string> WIN_RULES = new()
    {
        ["rock"] = "scissors",
        ["scissors"] = "paper",
        ["paper"] = "rock"
    };

    public static async Task<IResult> MapUtilPlay(
        string choice,
        string? xAccessToken
    )
    {
        choice = choice.ToLower();
        string? userId = null;
        // ===============================
        // 🔐 VERIFY TOKEN
        // ===============================
        if (!string.IsNullOrEmpty(xAccessToken))
        {

            var payload = JwtToken.Verify(xAccessToken);
            if (payload == null)
                return Results.Unauthorized(
                // ApiResponse.Response(401, "Invalid or expired token")
                );

            if (!payload.TryGetValue("user_id", out var userIdObj))
                return Results.Unauthorized(
                // ApiResponse.Response(401, "Invalid token payload")
                );
            userId = userIdObj.ToString();
        }



        // ===============================
        // 🎮 VALIDATE CHOICE
        // ===============================
        if (!OPTIONS.Contains(choice))
        {
            return Results.BadRequest(
                ApiResponse.Response(
                    400,
                    "Choice must be either 'rock', 'paper', or 'scissors'."
                )
            );
        }

        var random = new Random();
        var choiceComputer = OPTIONS[random.Next(OPTIONS.Length)];

        string status = "lose";
        string message = $"You lose! {choiceComputer.ToUpper()} beats {choice.ToUpper()}";

        if (choice == choiceComputer)
        {
            status = "draw";
            message = $"It's a draw! Both chose {choice.ToUpper()}";
        }
        else if (WIN_RULES[choice] == choiceComputer)
        {
            status = "win";
            message = $"You win! {choice.ToUpper()} beats {choiceComputer.ToUpper()}";
        }

        // ===============================
        // 🗄 DATABASE LOGIC
        // ===============================
        await using var conn = Configs.GetConnection();
        await conn.OpenAsync();
        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            int winStreak = 0;

            // 🔎 GET LAST WIN STREAK
            await using (var cmd = new NpgsqlCommand(
                $@"
                SELECT win_streak
                FROM {Configs.PostgresSchema}.tb_battles
                WHERE deleted_at IS NULL
                AND user_id = @user_id
                ORDER BY created_at DESC
                LIMIT 1
                ",
                conn, tx))
            {
                // cmd.Parameters.AddWithValue("user_id", Guid.Parse(userId!));
                cmd.Parameters.AddWithValue("user_id", userId!);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    winStreak = reader.GetInt32(0);
            }

            // 🔁 UPDATE STREAK
            if (status == "win")
                winStreak += 1;
            else if (status == "lose")
                winStreak = 0;

            var now = TimezoneNow.Now();

            // 📝 INSERT BATTLE
            await using (var cmdInsert = new NpgsqlCommand(
                $@"
                INSERT INTO {Configs.PostgresSchema}.tb_battles(
                    user_id,
                    created_at,
                    updated_at,
                    choice_user,
                    choice_computer,
                    ""result"",
                    win_streak
                ) VALUES (
                    @user_id,
                    @created_at,
                    @updated_at,
                    @choice_user,
                    @choice_computer,
                    @result,
                    @win_streak
                )
                ",
                conn, tx))
            {
                // cmdInsert.Parameters.AddWithValue("user_id", Guid.Parse(userId!));
                cmdInsert.Parameters.AddWithValue("user_id", userId!);
                cmdInsert.Parameters.AddWithValue("created_at", now);
                cmdInsert.Parameters.AddWithValue("updated_at", now);
                cmdInsert.Parameters.AddWithValue("choice_user", choice);
                cmdInsert.Parameters.AddWithValue("choice_computer", choiceComputer);
                cmdInsert.Parameters.AddWithValue("result", status);
                cmdInsert.Parameters.AddWithValue("win_streak", winStreak);

                await cmdInsert.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();

            return Results.Ok(
                ApiResponse.Response(200, data: new
                {
                    choice_user = choice,
                    choice_computer = choiceComputer,
                    result = new
                    {
                        status,
                        msg = message
                    },
                    win_streak = winStreak
                })
            );
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"\nError: {ex.Message}\n");

            return Results.InternalServerError(
                ApiResponse.Response(500)
            );
        }
    }
}
