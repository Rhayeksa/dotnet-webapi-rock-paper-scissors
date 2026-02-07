namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Login;

using System.ComponentModel.DataAnnotations;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util.Login;

public static class Login
{
    public class ReqLogin
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Username { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";
    }

    public static RouteGroupBuilder MapLogin(this RouteGroupBuilder user)
    {
        user.MapPost("/login", async Task<IResult> (ReqLogin body) =>
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

            return await UtilLogin.MapUtilLogin(
                username: body.Username,
                password: body.Password
            );
        })
        .WithName("LoginUser");

        return user;
    }
}

