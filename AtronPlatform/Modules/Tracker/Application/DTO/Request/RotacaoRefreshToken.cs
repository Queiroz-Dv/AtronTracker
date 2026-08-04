using System;

namespace Application.DTO.Request;

public sealed record RotacaoRefreshToken(
    string UsuarioCodigo,
    string RefreshTokenAtual,
    string NovoRefreshToken,
    DateTime NovaExpiracao);
