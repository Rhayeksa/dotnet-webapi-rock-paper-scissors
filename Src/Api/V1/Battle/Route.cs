namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Route;

using dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.Play;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.Battle.GetBattleInfoByUserId;

public static class Route
{
    public static RouteGroupBuilder MapRoutesBattle(this RouteGroupBuilder v1)
    {
        var result = v1.MapGroup(prefix: "/battle").WithTags(tags: "Battle");

        // Mapping
        result.MapGetBattleInfoByUserId();
        result.MapPlay();

        return result;
    }

}
