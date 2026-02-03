namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User;

public static class Route
{
    public static RouteGroupBuilder MapUserRoutes(this RouteGroupBuilder v1)
    {
        var result = v1.MapGroup(prefix: "/user").WithTags(tags: "User");

        // Mapping
        result.MapRegister();

        return result;
    }

}
