namespace dotnet_webapi_rock_paper_scissors.src.Common;

using Microsoft.AspNetCore.WebUtilities;

public static class ApiResponse
{
    public static object Response(
        int code = 200,
        string? msg = null,
        object? page = null,
        object? data = null
    )
    {
        // DateTime datetime = DateTime.Now;
        var datetime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        var status = ReasonPhrases.GetReasonPhrase(code);
        return new
        {
            datetime,
            status_code = code,
            status,
            message = msg ?? status,
            page,
            data
        };

    }
}
