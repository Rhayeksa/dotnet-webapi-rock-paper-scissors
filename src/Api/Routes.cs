namespace dotnet_webapi_rock_paper_scissors.src.Api.V1.User;

using dotnet_webapi_rock_paper_scissors.src.Common;

public static class Routes
{
    public static WebApplication MapApiRoutes(this WebApplication app)
    {
        // root endpoint
        // app.MapGet("/", () => new { message = "Hello .NET Web API!" });
        app.MapGet("/api", () =>
        {
            return Results.Ok(value: ApiResponse.Response(msg: "Welcome .NET Web API!"));
        });

        // versioned group: /api/v1
        var v1 = app.MapGroup("/api/v1");

        // route
        v1.MapUserRoutes();

        return app;
    }
}