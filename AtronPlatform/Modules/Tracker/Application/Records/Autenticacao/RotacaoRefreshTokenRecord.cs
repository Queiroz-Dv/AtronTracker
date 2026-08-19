using System;

namespace Application.Records.Autenticacao;

public sealed record RotacaoRefreshTokenRecord(
    string UsuarioCodigo,
    string RefreshTokenAtual,
    string NovoRefreshToken,
    DateTime NovaExpiracao);
