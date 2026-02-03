using System.ComponentModel.DataAnnotations;
using dotnet_webapi_rock_paper_scissors.src.Util;

public sealed class Request
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = "";

    [Required]
    [MinLength(3)]
    [MaxLength(30)]
    public string Username { get; set; } = "";

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = "";

    [Range(1, 250)]
    public int? Age { get; set; }

    [RegularExpression("^(M|F)$", ErrorMessage = "gender must be 'M' or 'F'")]
    public string? Gender { get; set; }

    [RegularExpression("^(administrator|player)$", ErrorMessage = "role must be 'administrator' or 'player'")]
    public string Role { get; set; } = "player";
}

public static class Register
{
    public static RouteGroupBuilder MapRegister(this RouteGroupBuilder user)
    {
        user.MapPost("/register", (Request body) =>
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

