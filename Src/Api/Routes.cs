namespace dotnet_webapi_rock_paper_scissors.Src.Api;

using dotnet_webapi_rock_paper_scissors.Src.Util;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User;

public static class Routes
{
    public static WebApplication MapApiRoutes(this WebApplication app)
    {
        app.MapGet("/api", () =>
        {
            return Results.Ok(value: ApiResponse.Response(msg: "Welcome to API .NET Game Rock Paper Scissor"));
        });

        // ---

        // versioned group: /api/v1
        var v1 = app.MapGroup("/api/v1");

        // route
        v1.MapUserRoutes();

        return app;
    }
}