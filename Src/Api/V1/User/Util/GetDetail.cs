namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Util.GetDetail;

using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;

public static class UtilGetDetail
{
    public static async Task<IResult> MapUtilGetDetail(string userId, string xAccessToken)
    {
        return Results.Ok(
            ApiResponse.Response(code: 200, msg: "OK Boss!")
        );
    }
}