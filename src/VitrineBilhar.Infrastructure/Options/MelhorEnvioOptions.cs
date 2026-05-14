namespace VitrineBilhar.Infrastructure.Options;

public sealed class MelhorEnvioOptions
{
    public const string SectionName = "MelhorEnvio";

    public string BaseUrl { get; set; } = "https://sandbox.melhorenvio.com.br/";
    public string Token { get; set; } = string.Empty;
}
