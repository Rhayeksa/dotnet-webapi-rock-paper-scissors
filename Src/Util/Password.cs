using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

public static class Password
{
    // Samakan dengan Python config
    private const int MemorySizeKb = 65536; // 64MB
    private const int Iterations = 3;       // time_cost
    private const int Parallelism = 2;      // parallelism
    private const int SaltSize = 16;        // salt length (bytes)
    private const int HashSize = 32;        // output length (bytes)
    private const int Version = 19;         // Argon2 v=19 (0x13)

    /// <summary>
    /// Return hash in Passlib-compatible format:
    /// $argon2id$v=19$m=65536,t=3,p=2$<salt_b64>$<hash_b64>
    /// </summary>
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = MemorySizeKb,
            Iterations = Iterations,
            DegreeOfParallelism = Parallelism
        };

        var hash = argon2.GetBytes(HashSize);

        // Passlib uses Base64 WITHOUT '=' padding
        var saltB64 = ToB64NoPadding(salt);
        var hashB64 = ToB64NoPadding(hash);

        return $"$argon2id$v={Version}$m={MemorySizeKb},t={Iterations},p={Parallelism}${saltB64}${hashB64}";
    }

    /// <summary>
    /// Verify Passlib-compatible Argon2id hash string.
    /// </summary>
    public static bool Verify(string password, string encoded)
    {
        // expected:
        // $argon2id$v=19$m=65536,t=3,p=2$SALT$HASH
        if (string.IsNullOrWhiteSpace(encoded)) return false;
        if (!encoded.StartsWith("$argon2id$")) return false;

        var parts = encoded.Split('$', StringSplitOptions.RemoveEmptyEntries);
        // parts:
        // [0]=argon2id
        // [1]=v=19
        // [2]=m=...,t=...,p=...
        // [3]=salt
        // [4]=hash
        if (parts.Length != 5) return false;

        // parse params
        var versionPart = parts[1]; // v=19
        var paramPart = parts[2];   // m=65536,t=3,p=2
        var saltPart = parts[3];
        var hashPart = parts[4];

        if (!versionPart.StartsWith("v=")) return false;
        if (!int.TryParse(versionPart.Substring(2), out var v)) return false;
        if (v != Version) return false;

        // defaults
        int m = 0, t = 0, p = 0;
        foreach (var item in paramPart.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = item.Split('=', 2);
            if (kv.Length != 2) continue;

            if (kv[0] == "m") int.TryParse(kv[1], out m);
            if (kv[0] == "t") int.TryParse(kv[1], out t);
            if (kv[0] == "p") int.TryParse(kv[1], out p);
        }

        if (m <= 0 || t <= 0 || p <= 0) return false;

        var salt = FromB64NoPadding(saltPart);
        var expectedHash = FromB64NoPadding(hashPart);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = m,
            Iterations = t,
            DegreeOfParallelism = p
        };

        var computed = argon2.GetBytes(expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(computed, expectedHash);
    }

    // ---- helpers (Passlib uses no padding) ----

    private static string ToB64NoPadding(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=');

    private static byte[] FromB64NoPadding(string b64)
    {
        // restore '=' padding
        var pad = 4 - (b64.Length % 4);
        if (pad is > 0 and < 4)
            b64 = b64 + new string('=', pad);

        return Convert.FromBase64String(b64);
    }
}
