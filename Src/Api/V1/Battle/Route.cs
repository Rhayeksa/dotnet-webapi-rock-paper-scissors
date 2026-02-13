namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Route;

using dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Play;

public static class Route
{
    public static RouteGroupBuilder MapRoutesBattle(this RouteGroupBuilder v1)
    {
        var result = v1.MapGroup(prefix: "/battle").WithTags(tags: "Battle");

        // Mapping
        result.MapPlay();

        return result;
    }

}
