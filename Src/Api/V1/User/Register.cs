using System.ComponentModel.DataAnnotations;
using dotnet_webapi_rock_paper_scissors.Src.Util;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util;


public static class Register
{
    public class ReqRegister
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

        [RegularExpression("^(player|administrator)$", ErrorMessage = "role must be 'administrator' or 'player'")]
        public string Role { get; set; } = "player";
    }

    public static RouteGroupBuilder MapRegister(this RouteGroupBuilder user)
    {
        user.MapPost("/register", (ReqRegister body) =>
        {
            var ctx = new ValidationContext(body);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(body, ctx, results, validateAllProperties: true))
            {
                return Results.BadRequest(ApiResponse.Response(
                    code: 400,
                    msg: results.First().ErrorMessage ?? "Validation error"
                ));
            }

            return UtilRegister.MapUtilRegister(
                name: body.Name,
                username: body.Username,
                password: body.Password,
                age: body.Age,
                gender: body.Gender,
                role: body.Role
            );
        })
        .WithName("RegisterUser");

        return user;
    }
}

