namespace dotnet_webapi_rock_paper_scissors.src.Api.V1.User.Util;

using dotnet_webapi_rock_paper_scissors.src.Util;

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
        try
        {

            // TODO: insert DB, hashing password, dsb

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
            Console.WriteLine($"Error (register post): {ex.Message}");
            return Results.InternalServerError(ApiResponse.Response(500, "Internal server error"));
        }

    }
}
