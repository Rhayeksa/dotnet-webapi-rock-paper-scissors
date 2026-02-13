namespace dotnet_webapi_rock_paper_scissors.Src.Api.Routes;

using dotnet_webapi_rock_paper_scissors.Src.Util.ApiResponse;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Route;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Route;

public static class Routes
{
    public static WebApplication MapApiRoutes(this WebApplication app)
    {
        app.MapGet("/api", () =>
        {
            return Results.Ok(value: ApiResponse.Response(msg: "Welcome to API .NET Game Rock Paper Scissor"));
        }).WithTags("default");

        // ---

        // versioned group: /api/v1
        var v1 = app.MapGroup("/api/v1");

        // route map
        v1.MapUserRoutes();
        v1.MapRoutesBattle();

        return app;
    }
}