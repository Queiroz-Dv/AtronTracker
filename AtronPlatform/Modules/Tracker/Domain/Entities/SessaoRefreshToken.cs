using System;

namespace Domain.Entities;

public sealed record SessaoRefreshToken(string UsuarioCodigo, DateTime ExpiraEm);

public sealed record RotacaoRefreshTokenHash(
    string UsuarioCodigo,
    string HashAtual,
    string NovoHash,
    DateTime NovaExpiracao);
