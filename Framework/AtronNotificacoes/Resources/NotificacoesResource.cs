using System.Globalization;
using System.Resources;

namespace AtronNotificacoes.Resources;

public static class NotificacoesResource
{
    private static readonly ResourceManager ResourceManager = new(
        "AtronNotificacoes.Resources.NotificacoesResource",
        typeof(NotificacoesResource).Assembly);

    public static string Erro_Publicacao => Obter(nameof(Erro_Publicacao));
    public static string Erro_TokenSemCodigoUsuario => Obter(nameof(Erro_TokenSemCodigoUsuario));
    public static string Log_FalhaPublicacao => Obter(nameof(Log_FalhaPublicacao));
    public static string Log_Publicacao => Obter(nameof(Log_Publicacao));
    public static string Saude_BancoDisponivel => Obter(nameof(Saude_BancoDisponivel));
    public static string Saude_BancoIndisponivel => Obter(nameof(Saude_BancoIndisponivel));
    public static string Saude_FalhaBanco => Obter(nameof(Saude_FalhaBanco));
    public static string Status_Preparado => Obter(nameof(Status_Preparado));

    private static string Obter(string chave) =>
        ResourceManager.GetString(chave, CultureInfo.CurrentUICulture)
        ?? throw new MissingManifestResourceException(
            $"Resource de notificações não encontrado: {chave}");
}
