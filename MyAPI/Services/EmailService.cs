using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class EmailService
{
    private readonly string _apiKey;

    public EmailService(IConfiguration config)
    {
        _apiKey = config["Brevo:ApiKey"] ?? Environment.GetEnvironmentVariable("Brevo__ApiKey")!;
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new Exception("Brevo API key is missing!");
    }

    public async Task SendOtpEmail(string toEmail, string otp)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.brevo.com/v3/smtp/email");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("api-key", _apiKey);

        var payload = new
        {
            sender = new { name = "Zentra App", email = "noreply@vanda.id.vn" },
            to = new[] { new { email = toEmail } },
            subject = "Mã OTP xác thực",
            htmlContent = GetOtpTemplate(otp)
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("", content);

        if (!response.IsSuccessStatusCode)
        {
            var respBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Brevo send email failed: {respBody}");
            throw new Exception($"Failed to send email via Brevo: {response.StatusCode}");
        }
    }

    private string GetOtpTemplate(string otp)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<style>
body {{ font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 0; }}
.container {{ max-width: 500px; margin: 40px auto; background: #ffffff; border-radius: 10px; padding: 30px; text-align: center; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
.title {{ font-size: 22px; font-weight: bold; margin-bottom: 10px; }}
.otp {{ font-size: 36px; font-weight: bold; color: #2d89ef; margin: 20px 0; letter-spacing: 6px; }}
.text {{ color: #555; font-size: 14px; line-height: 1.5; }}
.footer {{ margin-top: 30px; font-size: 12px; color: #999; }}
</style>
</head>
<body>
<div class='container'>
<div class='title'>Xác thực tài khoản</div>
<div class='text'>Bạn đang đăng ký tài khoản tại <b>Zentra App</b></div>
<div class='otp'>{otp}</div>
<div class='text'>Mã OTP có hiệu lực trong <b>5 phút</b>.<br/>Không chia sẻ mã này với bất kỳ ai.</div>
<div class='footer'>Nếu bạn không yêu cầu, hãy bỏ qua email này.</div>
</div>
</body>
</html>";
    }
}