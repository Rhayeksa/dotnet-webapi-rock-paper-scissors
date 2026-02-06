namespace dotnet_webapi_rock_paper_scissors.Src.Util;

public static class TimezoneNow
{
    public static DateTime Now(
        bool @default = false,
        string zoneInfo = "Asia/Jakarta"
    )
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(zoneInfo);
            var nowTz = TimeZoneInfo.ConvertTime(DateTime.Now, tz);

            if (@default)
                return DateTime.Now;

            return DateTime.SpecifyKind(nowTz, DateTimeKind.Unspecified);
        }
        catch
        {
            return DateTime.Now;
        }
    }
}
