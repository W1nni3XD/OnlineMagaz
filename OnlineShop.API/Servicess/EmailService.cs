using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OnlineShop.API.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string role)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            smtpSettings["SenderName"] ?? "OnlineShop",
            smtpSettings["SenderEmail"]!
        ));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Добро пожаловать в OnlineShop!";

        var roleText = role == "Seller"
            ? "Вы зарегистрированы как <strong>продавец</strong>. Теперь вы можете добавлять свои товары в каталог."
            : "Вы зарегистрированы как <strong>покупатель</strong>. Приятных покупок!";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                    <div style="background: #1976D2; padding: 24px; border-radius: 8px 8px 0 0;">
                        <h1 style="color: white; margin: 0; font-size: 24px;">🛍️ OnlineShop</h1>
                    </div>
                    <div style="background: #f9f9f9; padding: 32px; border-radius: 0 0 8px 8px; border: 1px solid #e0e0e0; border-top: none;">
                        <h2 style="color: #333; margin-top: 0;">Добро пожаловать!</h2>
                        <p style="color: #555; font-size: 16px;">
                            Ваш аккаунт <strong>{toEmail}</strong> успешно создан.
                        </p>
                        <p style="color: #555; font-size: 16px;">
                            {roleText}
                        </p>
                        <div style="margin-top: 32px; padding-top: 16px; border-top: 1px solid #e0e0e0;">
                            <p style="color: #999; font-size: 13px; margin: 0;">
                                Это письмо отправлено автоматически — отвечать на него не нужно.
                            </p>
                        </div>
                    </div>
                </div>
                """
        };

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                smtpSettings["Host"]!,
                int.Parse(smtpSettings["Port"] ?? "587"),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(smtpSettings["Username"]!, smtpSettings["Password"]!);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Welcome email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            // Не роняем регистрацию из-за ошибки email
            _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
        }
    }
}