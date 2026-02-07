namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.GetDetail;

// using System.ComponentModel.DataAnnotations;
using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util.GetDetail;
using Microsoft.AspNetCore.Mvc;

public static class GetDetail
{
    public static RouteGroupBuilder MapGetDetail(this RouteGroupBuilder user)
    {
        user.MapGet("/{userId}", async Task<IResult> (
            string userId,
            [FromHeader(Name = "x-access-token")] string xAccessToken
        ) =>
        {
            if (string.IsNullOrWhiteSpace(userId) || userId.Length < 3)
            {
                return Results.BadRequest(
                    ApiResponse.Response(400, "Invalid UserId")
                );
            }

            if (string.IsNullOrWhiteSpace(xAccessToken))
            {
                return Results.Unauthorized(
                // ApiResponse.Response(401, "Missing token")
                );
            }

            return await UtilGetDetail.MapUtilGetDetail(
                            userId: userId,
                            xAccessToken: xAccessToken
            );
        })
        .WithName("GetDetailUser");

        return user;
    }
}

