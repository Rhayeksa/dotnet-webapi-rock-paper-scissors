namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.GetDetail;

using System.ComponentModel.DataAnnotations;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util.GetDetail;
using Microsoft.AspNetCore.Mvc;

public static class GetDetail
{
    public class ReqUserGetDetail
    {
        [FromRoute]
        [Required]
        [MinLength(3)]
        public string UserId { get; set; } = "";

        [FromHeader(Name = "x-access-token")]
        [Required]
        public string Token { get; set; } = "";
    }
    public static RouteGroupBuilder MapGetDetail(this RouteGroupBuilder user)
    {

        user.MapGet("/{userId}", async Task<IResult> ([AsParameters] ReqUserGetDetail req) =>
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

            return await UtilGetDetail.MapUtilGetDetail(
                userId: req.UserId,
                xAccessToken: req.Token
            );
        })
        .WithName("GetDetailUser");
        // });

        return user;
    }
}

