using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace argosAPI.Helpers;

/// <summary>Regras compartilhadas pelas rotas e pela autenticação.</summary>
public static class ApiRules
{
    public static string NormalizeCnpj(string value) =>
        new(value.Where(char.IsDigit).ToArray());

    public static bool IsValidCnpj(string value)
    {
        var cnpj = NormalizeCnpj(value);
        if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1) return false;

        int[] weights1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] weights2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        return Check(cnpj, weights1, 12) && Check(cnpj, weights2, 13);
    }

    private static bool Check(string value, int[] weights, int length)
    {
        var sum = 0;
        for (var i = 0; i < length; i++) sum += (value[i] - '0') * weights[i];
        var digit = sum % 11 < 2 ? 0 : 11 - sum % 11;
        return digit == value[length] - '0';
    }

    public static bool TryDate(string value, out DateOnly date) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date);

    public static bool IsValidProcess(string value) =>
        System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{10}\.\d{6}/\d{4}-\d{2}$");

    /// <summary>Compatibilidade temporária com usuários</summary>
    public static string Sha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static DateOnly TodayBrasilia()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }
}
