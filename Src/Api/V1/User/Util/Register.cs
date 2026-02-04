namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util;

using dotnet_webapi_rock_paper_scissors.src.Configs;
using dotnet_webapi_rock_paper_scissors.Src.Util;
using Npgsql;
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
            tx = conn.BeginTransaction();

            // 1) check username exists
            using (var cmd = new NpgsqlCommand(
                $@"
                SELECT count(1) AS count
                FROM {Configs.PostgresSchema}.tb_users
                WHERE deleted_at IS NULL
                AND username = @username
                "
                , conn, tx))
            {
                cmd.Parameters.AddWithValue("username", username);
                using var reader = cmd.ExecuteReader();
                reader.Read();

                if ((long)reader["count"] > 0)
                    return Results.BadRequest(ApiResponse.Response(400, "username already exist"));
            }

            // 2) INSERT user baru
            var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var userId = Guid.NewGuid();

            using (var cmd = new NpgsqlCommand(
                $@"
                INSERT INTO {Configs.PostgresSchema}.tb_users(
                    user_id,
                    created_at,
                    updated_at,
                    username,
                    ""password"",
                    ""name"",
                    age,
                    gender,
                    role
                ) VALUES (
                    @user_id,
                    @created_at,
                    @updated_at,
                    @username,
                    @password,
                    @name,
                    @age,
                    @gender,
                    @role
                )
                ",
                conn, tx))
            {
                cmd.Parameters.AddWithValue("user_id", userId);
                cmd.Parameters.AddWithValue("created_at", now);
                cmd.Parameters.AddWithValue("updated_at", now);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", Password.Hash(password));
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("role", role);

                // nullable harus pakai DBNull.Value
                cmd.Parameters.AddWithValue("age", (object?)age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("gender", (object?)gender ?? DBNull.Value);

                var affected = cmd.ExecuteNonQuery();
                // if (affected != 1)
                //     throw new Exception("Insert failed: affected rows != 1");
            }

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
