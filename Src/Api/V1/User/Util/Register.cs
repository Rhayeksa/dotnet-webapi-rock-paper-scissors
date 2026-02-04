namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util;

using dotnet_webapi_rock_paper_scissors.Src.Util;
using Npgsql;
using dotnet_webapi_rock_paper_scissors.src.Configs;
public static class UtilRegister
{
    public static object MapUtilRegister(
        string name,
        string username,
        string password,
        int? age = null,
        string? gender = null,
        string role = "player"
    )
    {
        NpgsqlConnection? conn = null;
        NpgsqlTransaction? tx = null;
        try
        {

            conn = Configs.GetConnection();
            conn.Open();

            // 🔥 transaction begin
            tx = conn.BeginTransaction();
            // TODO: insert DB, hashing password, dsb
            // 1) check username exists

            // var count = conn.exceute(
            //     $@"
            //     SELECT count(1)
            //     FROM {Configs.PostgresSchema}.tb_users
            //     WHERE deleted_at IS NULL
            //     AND username = @username
            //     ",
            //     new() { ["username"] = username }
            // );
            var cmd = new NpgsqlCommand(
                $@"
                SELECT role, gender
                FROM {Configs.PostgresSchema}.tb_users
                WHERE deleted_at IS NULL
                AND username = @username
                "
                , conn, tx);
            cmd.Parameters.AddWithValue("username", username);

            string? roleDb = null;
            string? genderDb = null;

            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    return Results.BadRequest(ApiResponse.Response(404, "user not found"));
                roleDb = reader["role"]?.ToString();
                genderDb = reader["gender"]?.ToString();
            } // reader dipastikan close/dispose disini

            Console.WriteLine($">>> role: {roleDb}");
            Console.WriteLine($">>> gender: {genderDb}");

            // ---

            // result.Read();
            // // Console.WriteLine($">>> result: {result}");
            // var a = result["role"];
            // var b = result["gender"];
            // Console.WriteLine($">>> a: {a}");
            // Console.WriteLine($">>> b: {b}");

            // if (result["count"] > 0)
            //     return Results.BadRequest(ApiResponse.Response(400, "username already exists"));

            tx.Commit();
            return Results.Created(
                uri: "/api/v1/user/register",
                value: ApiResponse.Response(
                    code: 201,
                    msg: "User registered"
                )
            );
        }
        catch (Exception ex)
        {
            tx?.Rollback();
            Console.WriteLine($"Error (register post): {ex.Message}");
            return Results.InternalServerError(ApiResponse.Response(code: 500));
        }
        finally
        {
            conn?.Close();
        }

    }
}
