namespace AtronPlatform.WebApi.Security;

// Exceção apenas à exigência de empresa. A autenticação da rota continua obrigatória.
[AttributeUsage(AttributeTargets.Method)]
public sealed class PermitirSemEmpresaAttribute : Attribute;
