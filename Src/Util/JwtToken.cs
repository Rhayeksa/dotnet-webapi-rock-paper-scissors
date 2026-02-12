namespace dotnet_webapi_rock_paper_scissors.Src.Util.JwtToken;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using dotnet_webapi_rock_paper_scissors.Src.Configs;
using dotnet_webapi_rock_paper_scissors.Src.Util.TimezoneNow;

public static class JwtToken
{
    public static Dictionary<string, string> Generate(
        Dictionary<string, object> payload
    )
    {
        var now = TimezoneNow.Now(true);
        var expires = now.AddHours(Configs.JwtAccessTokenExpireHours);

        var claims = payload.Select(kv =>
            new Claim(kv.Key, kv.Value.ToString()!)
        ).ToList();

        claims.Add(new Claim(JwtRegisteredClaimNames.Iat,
            ((DateTimeOffset)now).ToUnixTimeSeconds().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Exp,
            ((DateTimeOffset)expires).ToUnixTimeSeconds().ToString()));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configs.JwtSecretKey)
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                key,
                Configs.JwtAlgorithm
            )
        );

        return new Dictionary<string, string>
        {
            ["x-access-token"] = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }

    public static Dictionary<string, object>? Verify(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configs.JwtSecretKey);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // biar strict (tidak ada toleransi waktu)
            };

            var principal = handler.ValidateToken(
                token,
                parameters,
                out SecurityToken validatedToken
            );

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims
                .Where(c =>
                    c.Type != JwtRegisteredClaimNames.Exp &&
                    c.Type != JwtRegisteredClaimNames.Iat
                )
                .ToDictionary(
                    c => c.Type,
                    c => (object)c.Value
                );
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine("Token expired");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid token: {ex.Message}");
            return null;
        }
    }
}
