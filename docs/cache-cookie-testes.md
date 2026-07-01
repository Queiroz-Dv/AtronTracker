# Testes do fluxo de cache e cookie

Este roteiro valida o fluxo implementado para cache, cookie de diagnostico, cookie de refresh token e sinais basicos da sessao.

## 1. Preparar configuracao

No `AtronTracker/WebApi/appsettings.Development.json`, confirme:

```json
"Diagnostico": {
  "Habilitado": true,
  "Chave": ""
},
"Cache": {
  "Provider": "Memory",
  "JsonFile": {
    "Diretorio": "App_Data/cache-json"
  }
}
```

Para exigir chave no diagnostico, preencha `Diagnostico:Chave` e envie o mesmo valor no header `X-DIAGNOSTICO-CHAVE`.

## 2. Subir a API

Na raiz do repositorio:

```powershell
dotnet run --project AtronTracker\WebApi\WebApi.csproj
```

Use a URL HTTPS exibida no terminal. Cookies marcados como `Secure` podem ser ignorados pelo navegador ou cliente HTTP se a chamada for feita por HTTP.

## 3. Testar status do diagnostico

Chame:

```http
GET /api/diagnostico/status
```

Resultado esperado:

- `ativo: true`
- `cacheProviderConfigurado: "Memory"` ou `"JsonFile"`
- `cacheDistribuido: false`
- `cookieDiagnostico: "ATRON_DIAGNOSTICO_COOKIE"`
- `requerChave: false`, se `Diagnostico:Chave` estiver vazio

## 4. Testar cache com ciclo completo

Chame:

```http
POST /api/diagnostico/cache/testar
Content-Type: application/json

{
  "chave": "teste-cache-autoteste",
  "valor": "valor do autoteste",
  "ttlSegundos": 120
}
```

Resultado esperado:

- `sucesso: true`
- `gravacaoOk: true`
- `leituraOk: true`
- `remocaoOk: true`
- `implementacaoCache` deve refletir o provider ativo

## 5. Testar cache manualmente

Grave:

```http
POST /api/diagnostico/cache/gravar
Content-Type: application/json

{
  "chave": "teste-cache",
  "valor": "valor de diagnostico",
  "ttlSegundos": 120
}
```

Leia:

```http
GET /api/diagnostico/cache/ler/teste-cache
```

Resultado esperado na leitura:

- `status: "encontrado"`
- `valorEncontrado: true`

Remova:

```http
DELETE /api/diagnostico/cache/remover/teste-cache
```

Leia novamente. Resultado esperado:

- `status: "nao_encontrado"`
- `valorEncontrado: false`

## 6. Testar provider JsonFile

Altere temporariamente:

```json
"Cache": {
  "Provider": "JsonFile",
  "JsonFile": {
    "Diretorio": "App_Data/cache-json"
  }
}
```

Reinicie a API e repita os passos 3, 4 e 5.

Resultado esperado:

- `cacheProviderConfigurado: "JsonFile"`
- `implementacaoCache: "JsonFileCacheService"`
- `cacheDiretorioArquivoJson` preenchido
- Durante um item gravado e ainda nao removido, o diretorio configurado deve conter um arquivo `.json`
- Apos remover o item, o arquivo correspondente deve desaparecer

Observacao: este provider sobrevive a reinicio enquanto o item nao expirar e nao for removido, mas continua sendo local da instancia.

## 7. Testar cookie de diagnostico

Grave:

```http
POST /api/diagnostico/cookie/gravar
Content-Type: application/json

{
  "valor": "cookie de diagnostico",
  "minutosExpiracao": 10
}
```

Leia:

```http
GET /api/diagnostico/cookie/ler
```

Resultado esperado:

- `cookieEncontrado: true`
- `consegueDesproteger: true`
- `fingerprint` preenchido

Remova:

```http
DELETE /api/diagnostico/cookie/remover
```

Leia novamente. Resultado esperado:

- `cookieEncontrado: false`

## 8. Testar sinais da sessao

Chame:

```http
GET /api/diagnostico/sessao/sinais
XUSRCD: CODIGO_DO_USUARIO
```

Antes do login, o esperado normalmente e:

- `possuiHeaderUsuario: true`
- `possuiCookieRefreshToken: false`
- `cacheAcessoEncontrado: false`

Apos login, usando o mesmo cliente HTTP que preservou o cookie:

- `possuiCookieRefreshToken: true`
- `nomeCookieRefreshToken` deve seguir o formato `CODIGO_DO_USUARIOREFRESHTOKEN`
- `cacheAcessoEncontrado` pode ficar `true` quando o fluxo de login/sessao gravar os dados complementares do usuario

## 9. Testar login e refresh token

Use `AtronTracker/WebApi/Requests/Acesso.http`.

Fluxo esperado:

1. Execute login em `/api/acesso/login`.
2. Confirme que a resposta trouxe o access token.
3. Confirme que o cliente HTTP recebeu cookie de refresh token.
4. Execute `/api/acesso/RefreshToken` com header `XUSRCD`.
5. Resultado esperado: novo access token retornado e cookie de refresh token rotacionado.
6. Execute `/api/acesso/Desconectar` com `XUSRCD` e `Authorization: Bearer`.
7. Execute refresh novamente. Resultado esperado: falha por refresh token invalido ou ausente.

## 10. Testar expiracao

Para cache:

1. Grave item com `ttlSegundos: 3`.
2. Leia imediatamente. Deve encontrar.
3. Aguarde mais de 3 segundos.
4. Leia novamente. Deve retornar nao encontrado.

Para cookie de diagnostico:

1. Grave cookie com `minutosExpiracao: 1`.
2. Leia imediatamente. Deve encontrar.
3. Aguarde expirar.
4. Leia novamente. O cliente pode deixar de enviar o cookie, retornando `cookieEncontrado: false`.

## 11. Criterios de aceite

- `Memory` compila e passa nos testes de cache.
- `JsonFile` compila e passa nos mesmos testes de cache.
- O diagnostico mostra o provider ativo corretamente.
- Cookie de diagnostico e protegido por Data Protection.
- Refresh token fica apenas em cookie protegido, nao no corpo da resposta de login.
- Refresh token e validado contra banco e rotacionado no refresh.
- Logout remove cookie e redefine refresh token no banco.
- Alteracoes de usuario e perfil invalidam caches de acesso/token.
