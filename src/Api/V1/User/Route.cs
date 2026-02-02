using dotnet_webapi_rock_paper_scissors.src.Api.V1.User;

namespace dotnet_webapi_rock_paper_scissors.src.Api.V1.User;

public static class Route
{
    public static RouteGroupBuilder MapUserRoutes(this RouteGroupBuilder v1)
    {
        var user = v1.MapGroup(prefix: "/user").WithTags(tags: "User x");

        user.MapRegister();

        return user;
    }
}
