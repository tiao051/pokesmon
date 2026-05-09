using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace backend.Application.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly string _templatesPath;

    public EmailService(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _templatesPath = Path.Combine(env.ContentRootPath, "Application", "Templates", "Emails");
    }

    private async Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> placeholders)
    {
        string bodyPath = Path.Combine(_templatesPath, $"{templateName}.html");
        string bodyContent = await File.ReadAllTextAsync(bodyPath);

        foreach (var kvp in placeholders)
        {
            bodyContent = bodyContent.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }

        string layoutPath = Path.Combine(_templatesPath, "_Layout.html");
        string layoutContent = await File.ReadAllTextAsync(layoutPath);

        layoutContent = layoutContent.Replace("{{bodyContent}}", bodyContent)
                                     .Replace("{{year}}", DateTime.UtcNow.Year.ToString());

        return layoutContent;
    }

    // --------------------------------------------------------------------------------
    // 1. ACCOUNT ACTIVATION PIN (Register)
    // --------------------------------------------------------------------------------
    public async Task SendRegistrationPinEmailAsync(string toEmail, string pin)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "pin", pin }
        };
        string content = await LoadTemplateAsync("RegistrationPin", placeholders);
        await SendThemedEmailAsync(toEmail, "Activate Your PokéGogh Account", content);
    }

    // --------------------------------------------------------------------------------
    // 2. FORGOT PASSWORD PIN
    // --------------------------------------------------------------------------------
    public async Task SendResetPasswordPinEmailAsync(string toEmail, string pin)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "pin", pin }
        };
        string content = await LoadTemplateAsync("ResetPasswordPin", placeholders);
        await SendThemedEmailAsync(toEmail, "Reset Your PokéGogh Password", content);
    }

    // --------------------------------------------------------------------------------
    // 3. PRE-ORDER CONFIRMATION
    // --------------------------------------------------------------------------------
    public async Task SendPreOrderConfirmationAsync(string toEmail, string username, string orderId, string itemName)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "username", username },
            { "orderId", orderId },
            { "itemName", itemName }
        };
        string content = await LoadTemplateAsync("PreOrderConfirmation", placeholders);
        await SendThemedEmailAsync(toEmail, "Pre-order Confirmed - PokéGogh", content);
    }

    // --------------------------------------------------------------------------------
    // 4. PAYMENT SUCCESS
    // --------------------------------------------------------------------------------
    public async Task SendPaymentSuccessAsync(string toEmail, string username, string orderId, decimal amount)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "username", username },
            { "orderId", orderId },
            { "amount", amount.ToString("F2") }
        };
        string content = await LoadTemplateAsync("PaymentSuccess", placeholders);
        await SendThemedEmailAsync(toEmail, "Payment Successful - PokéGogh", content);
    }

    // --------------------------------------------------------------------------------
    // 5. DELIVERY SUCCESS
    // --------------------------------------------------------------------------------
    public async Task SendDeliverySuccessAsync(string toEmail, string username, string orderId)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "username", username },
            { "orderId", orderId }
        };
        string content = await LoadTemplateAsync("DeliverySuccess", placeholders);
        await SendThemedEmailAsync(toEmail, "Your Order Has Arrived! - PokéGogh", content);
    }

    // --------------------------------------------------------------------------------
    // CORE SENDER LOGIC
    // --------------------------------------------------------------------------------
    private async Task SendThemedEmailAsync(string toEmail, string subject, string finalHtmlContent)
    {
        var email = new MimeMessage();
        var smtpUser = _config["SMTP_USER"];
        var smtpPass = _config["SMTP_PASS"];
        
        if (string.IsNullOrEmpty(smtpUser) || smtpUser.Contains("xxxxx") || string.IsNullOrEmpty(smtpPass))
        {
            Console.WriteLine($"\n--- [Mock Email Sent] ---");
            Console.WriteLine($"To: {toEmail}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine(finalHtmlContent);
            Console.WriteLine($"-------------------------\n");
            return;
        }

        email.From.Add(MailboxAddress.Parse(smtpUser));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = finalHtmlContent };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending themed email: {ex.Message}");
        }
    }
}
