namespace dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Route;

using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Register;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.Login;
using dotnet_webapi_rock_paper_scissors.Src.Api.V1.User.GetDetail;

public static class Route
{
    public static RouteGroupBuilder MapUserRoutes(this RouteGroupBuilder v1)
    {
        var result = v1.MapGroup(prefix: "/user").WithTags(tags: "User");

        // Mapping
        result.MapRegister();
        result.MapLogin();
        result.MapGetDetail();

        return result;
    }

}
