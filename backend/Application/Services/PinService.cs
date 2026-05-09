using System.Security.Cryptography;
using backend.Core.Common;
using backend.Core.Entities;
using MongoDB.Driver;

namespace backend.Application.Services;

public class PinService
{
    private readonly ILogger<PinService> _logger;

    public PinService(ILogger<PinService> logger)
    {
        _logger = logger;
    }

    public string GenerateSecurePin()
    {
        var max = (int)Math.Pow(10, Constants.OtpLengthDigits);
        var n = RandomNumberGenerator.GetInt32(0, max);
        return n.ToString().PadLeft(Constants.OtpLengthDigits, '0');
    }

    public string HashPin(string pin) => BCrypt.Net.BCrypt.HashPassword(pin);

    public bool VerifyPin(string pin, string hash) => BCrypt.Net.BCrypt.Verify(pin, hash);

    public async Task<OtpRecord> IssueAsync(
        IMongoCollection<OtpRecord> collection,
        string email,
        CancellationToken ct = default)
    {
        await collection.DeleteManyAsync(p => p.Email == email, ct);

        var pin = GenerateSecurePin();
        var record = new OtpRecord
        {
            Email = email,
            PinHash = HashPin(pin),
            ExpiresAt = DateTime.UtcNow.AddMinutes(Constants.OtpExpiryMinutes)
        };
        await collection.InsertOneAsync(record, cancellationToken: ct);

        return new OtpRecord
        {
            Id = record.Id,
            Email = record.Email,
            PinHash = pin,
            CreatedAt = record.CreatedAt,
            ExpiresAt = record.ExpiresAt
        };
    }

    public async Task<OtpVerifyResult> VerifyAsync(
        IMongoCollection<OtpRecord> collection,
        string email,
        string pin,
        CancellationToken ct = default)
    {
        var record = await collection.Find(p => p.Email == email).FirstOrDefaultAsync(ct);
        if (record is null) return OtpVerifyResult.NotFound;
        if (record.ExpiresAt < DateTime.UtcNow) return OtpVerifyResult.Expired;
        if (record.Attempts >= Constants.OtpMaxAttempts) return OtpVerifyResult.TooManyAttempts;

        if (!VerifyPin(pin, record.PinHash))
        {
            await collection.UpdateOneAsync(
                p => p.Id == record.Id,
                Builders<OtpRecord>.Update.Inc(p => p.Attempts, 1),
                cancellationToken: ct);
            return OtpVerifyResult.Invalid;
        }

        return OtpVerifyResult.Ok;
    }

    public Task ConsumeAsync(
        IMongoCollection<OtpRecord> collection,
        string email,
        CancellationToken ct = default)
        => collection.DeleteManyAsync(p => p.Email == email, ct);
}

public enum OtpVerifyResult { Ok, NotFound, Expired, Invalid, TooManyAttempts }
