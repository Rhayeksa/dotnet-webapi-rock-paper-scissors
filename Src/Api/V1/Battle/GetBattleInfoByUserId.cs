namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.GetBattleInfoByUserId
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
    // using dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Util.GetBattleInfoByUserId;
    using Microsoft.AspNetCore.Mvc;

    public static class GetBattleInfoByUserId
    {
        public class Req
        {
            [FromRoute]
            [MinLength(3)]
            public string UserId { get; set; } = "";

            [FromHeader(Name = "x-access-token")]
            [Required]
            public string Token { get; set; } = "";

        }
        public static RouteGroupBuilder MapGetBattleInfoByUserId(this RouteGroupBuilder route)
        {
            route.MapGet("/user/{userId}", async Task<IResult> ([AsParameters] Req req) =>
            {
                var ctx = new ValidationContext(req);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(req, ctx, results, validateAllProperties: true))
                {
                    return Results.BadRequest(ApiResponse.Response(
                        code: 400,
                        msg: results.First().ErrorMessage ?? "Validation error"
                    ));
                }

                // return await UtilGetBattleInfoByUserId.MapUtilGetBattleInfoByUserId(
                //     userId: req.UserId,
                //     xAccessToken: req.Token
                // );
                return Results.Ok(ApiResponse.Response(
                code: 200, msg: "Let's play", data: new { user_id = req.UserId }
                ));

            })
            .WithName("GetBattleInfoByUserId");

            return route;
        }
    }
}