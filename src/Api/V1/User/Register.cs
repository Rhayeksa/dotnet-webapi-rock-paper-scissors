using dotnet_webapi_rock_paper_scissors.src.Common;

namespace dotnet_webapi_rock_paper_scissors.src.Api.V1.User;

public static class Register
{
    public static RouteGroupBuilder MapRegister(this RouteGroupBuilder user)
    {
        user.MapPost("/register", (RegisterRequest body) =>
        {
            try
            {
                // contoh validasi sederhana
                if (string.IsNullOrWhiteSpace(body.Username))
                    return Results.BadRequest(ApiResponse.Response(400, "username is required"));

                if (string.IsNullOrWhiteSpace(body.Password))
                    return Results.BadRequest(ApiResponse.Response(400, "password is required"));

                // TODO: insert DB, hashing password, dsb

                return Results.Created(
                    uri: "/api/v1/user/register",
                    value: ApiResponse.Response(
                        code: 201,
                        msg: "User registered",
                        data: new
                        {
                            name = body.Name,
                            username = body.Username
                        }
                    )
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Results.InternalServerError(ApiResponse.Response(500, "Internal server error"));
            }
        })
        .WithName("RegisterUser");

        return user;
    }
}

public sealed class RegisterRequest
{
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string Role { get; set; } = "player";
}
