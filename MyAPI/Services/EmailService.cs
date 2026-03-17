using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailService
{
    private readonly string _apiKey;

    public EmailService(IConfiguration config)
    {
        _apiKey = config["SendGrid:ApiKey"]!;
    }

    public async Task SendOtpEmail(string toEmail, string otp)
    {
        var client = new SendGridClient(_apiKey);

        var from = new EmailAddress("noreply@vanda.id.vn", "Zentra App");
        var to = new EmailAddress(toEmail);

        var subject = "Mã OTP xác thực";

        var html = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f4f6f8;
      margin: 0;
      padding: 0;
    }}
    .container {{
      max-width: 500px;
      margin: 40px auto;
      background: #ffffff;
      border-radius: 10px;
      padding: 30px;
      text-align: center;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }}
    .title {{
      font-size: 22px;
      font-weight: bold;
      margin-bottom: 10px;
    }}
    .otp {{
      font-size: 36px;
      font-weight: bold;
      color: #2d89ef;
      margin: 20px 0;
      letter-spacing: 5px;
    }}
    .text {{
      color: #555;
      font-size: 14px;
    }}
    .footer {{
      margin-top: 30px;
      font-size: 12px;
      color: #999;
    }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='title'>Xác thực tài khoản</div>

    <div class='text'>
      Bạn đang đăng ký tài khoản tại <b>Zentra App</b>
    </div>

    <div class='otp'>{otp}</div>

    <div class='text'>
      Mã OTP có hiệu lực trong <b>5 phút</b>.<br/>
      Không chia sẻ mã này với bất kỳ ai.
    </div>

    <div class='footer'>
      Nếu bạn không yêu cầu, hãy bỏ qua email này.
    </div>
  </div>
</body>
</html>
";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, null, html);

        await client.SendEmailAsync(msg);
    }
}