using System.Security.Cryptography;
using System.Text;
using StackExchange.Redis;

public class OtpService
{
    private readonly IDatabase _redis;

    public OtpService(RedisService redisService)
    {
        _redis = redisService.Db;
    }

    private string OtpKey(string email) => $"otp:{email}";
    private string AttemptKey(string email) => $"otp_attempt:{email}";
    private string CooldownKey(string email) => $"otp_cd:{email}";
    private string TokenKey(string token) => $"verify:{token}";

    // ===== 1. Generate OTP =====
    public async Task<(bool Success, string Message, string? Otp)> GenerateOtpAsync(string email)
    {
        try
        {
            var cdKey = CooldownKey(email);

            // check cooldown
            if (await _redis.KeyExistsAsync(cdKey))
            {
                var ttl = await _redis.KeyTimeToLiveAsync(cdKey);
                var seconds = ttl?.TotalSeconds ?? 0;

                return (false, $"Vui lòng đợi {Math.Ceiling(seconds)} giây để gửi lại OTP", null);
            }

            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // lưu OTP 5 phút
            await _redis.StringSetAsync(OtpKey(email), otp, TimeSpan.FromMinutes(5));

            // cooldown 60s
            await _redis.StringSetAsync(cdKey, "1", TimeSpan.FromSeconds(60));

            // reset attempt
            await _redis.KeyDeleteAsync(AttemptKey(email));

            return (true, "OTP created", otp);
        }
        catch
        {
            return (false, "Không thể tạo OTP, thử lại sau", null);
        }
    }

    // ===== 2. Verify OTP =====
    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        var storedOtp = await _redis.StringGetAsync(OtpKey(email));

        if (storedOtp.IsNullOrEmpty)
            return false;

        // check lock (max 5 lần sai)
        var attempts = await _redis.StringGetAsync(AttemptKey(email));
        int attemptCount = attempts.IsNull ? 0 : (int)attempts;

        if (attemptCount >= 5)
            throw new Exception("Bạn đã nhập sai quá nhiều lần.");

        if (storedOtp != otp)
        {
            await _redis.StringIncrementAsync(AttemptKey(email));
            await _redis.KeyExpireAsync(AttemptKey(email), TimeSpan.FromMinutes(5));
            return false;
        }

        // đúng → xoá OTP
        await _redis.KeyDeleteAsync(OtpKey(email));
        await _redis.KeyDeleteAsync(AttemptKey(email));

        return true;
    }

    // ===== 3. Generate Register Token =====
    public async Task<string> GenerateRegisterTokenAsync(string email)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        await _redis.StringSetAsync(
            TokenKey(token),
            email,
            TimeSpan.FromMinutes(10)
        );

        return token;
    }

    // ===== 4. Get Email từ Token =====
    public async Task<string?> GetEmailFromTokenAsync(string token)
    {
        var email = await _redis.StringGetAsync(TokenKey(token));

        if (email.IsNullOrEmpty)
            return null;

        return email!;
    }

    // ===== 5. Remove Token =====
    public async Task RemoveTokenAsync(string token)
    {
        await _redis.KeyDeleteAsync(TokenKey(token));
    }
}