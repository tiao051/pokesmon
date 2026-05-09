using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace backend.Application.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly string _templatesPath;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, IWebHostEnvironment env, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
        _templatesPath = Path.Combine(env.ContentRootPath, "Application", "Templates", "Emails");
    }

    public Task SendRegistrationPinEmailAsync(string toEmail, string pin)
        => SendTemplatedEmailAsync(toEmail, "Activate Your PokéGogh Account", "RegistrationPin",
            new() { ["pin"] = pin });

    public Task SendResetPasswordPinEmailAsync(string toEmail, string pin)
        => SendTemplatedEmailAsync(toEmail, "Reset Your PokéGogh Password", "ResetPasswordPin",
            new() { ["pin"] = pin });

    private async Task SendTemplatedEmailAsync(
        string toEmail,
        string subject,
        string templateName,
        Dictionary<string, string> placeholders)
    {
        var content = await LoadTemplateAsync(templateName, placeholders);
        await SendAsync(toEmail, subject, content);
    }

    private async Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> placeholders)
    {
        var bodyPath = Path.Combine(_templatesPath, $"{templateName}.html");
        var body = await File.ReadAllTextAsync(bodyPath);

        foreach (var kvp in placeholders)
            body = body.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);

        var layoutPath = Path.Combine(_templatesPath, "_Layout.html");
        var layout = await File.ReadAllTextAsync(layoutPath);

        return layout
            .Replace("{{bodyContent}}", body)
            .Replace("{{year}}", DateTime.UtcNow.Year.ToString());
    }

    private async Task SendAsync(string toEmail, string subject, string htmlContent)
    {
        var smtpUser = _config["SMTP_USER"];
        var smtpPass = _config["SMTP_PASS"];
        var mockMode = _config.GetValue<bool>("EMAIL_MOCK_MODE");

        if (mockMode || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
        {
            _logger.LogInformation("[Mock Email] To={To} Subject={Subject}", toEmail, subject);
            return;
        }

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(smtpUser));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new BodyBuilder { HtmlBody = htmlContent }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(smtpUser, smtpPass);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
