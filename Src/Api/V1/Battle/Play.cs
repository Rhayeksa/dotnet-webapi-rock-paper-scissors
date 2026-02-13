namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Play;

using System.ComponentModel.DataAnnotations;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Util.Play;
using Microsoft.AspNetCore.Mvc;

public static class Play
{
    private static readonly HashSet<string> VALID_CHOICES =
            new(["rock", "paper", "scissors"]);
    public class ReqBodyPlay
    {
        [Required]
        public string Choice { get; set; } = "rock";
    }

    public static RouteGroupBuilder MapPlay(this RouteGroupBuilder route)
    {
        route.MapPost(pattern: "/", async Task<IResult> (
            [FromBody] ReqBodyPlay req,
            [FromHeader(Name = "x-access-token")] string token
        ) =>
        {
            var ctx = new ValidationContext(req);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(req, ctx, results, validateAllProperties: true))
            {
                return Results.BadRequest(
                    ApiResponse.Response(
                        400,
                        results.First().ErrorMessage ?? "Validation error"
                    )
                );
            }

            var choice = req.Choice.ToLowerInvariant();
            if (!VALID_CHOICES.Contains(choice))
            {
                return Results.BadRequest(
                    ApiResponse.Response(
                        400,
                        "choice must be rock, paper, or scissors"
                    )
                );
            }
            // panggil util
            return await UtilPlay.MapUtilPlay(
                choice: choice,
                xAccessToken: token
            );
            // return Results.Ok(ApiResponse.Response(
            //     code: 200, msg: "Let's play", data: new { pilihan = choice }
            // ));

        })
        .RequireRateLimiting("battle-limit") // 🔥 setara limiter.limit("15/minute")
        .WithName("PlayBattle")
        .WithSummary("Play battle")
        .WithDescription("Let's play rock paper scissors");

        return route;
    }
}
