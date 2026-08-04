# Headers de segurança do frontend no Render

O frontend é publicado como Static Site. Nesse tipo de serviço, os headers HTTP são configurados no Dashboard do Render.

Adicionar as regras abaixo para o caminho `/*`:

| Header | Valor |
| --- | --- |
| `Content-Security-Policy` | `default-src 'self'; base-uri 'self'; object-src 'none'; form-action 'self'; frame-ancestors 'none'; frame-src 'none'; script-src 'self'; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' data: https://fonts.gstatic.com; img-src 'self' data:; connect-src 'self' https://atronplatform.onrender.com; upgrade-insecure-requests` |
| `X-Frame-Options` | `DENY` |
| `X-Content-Type-Options` | `nosniff` |
| `Referrer-Policy` | `no-referrer` |
| `Permissions-Policy` | `camera=(), microphone=(), geolocation=(), payment=(), usb=()` |

Depois da publicação, validar a página inicial e uma rota interna com `curl -I`. O arquivo `index.html` também contém CSP e política de referência como proteção complementar, mas `frame-ancestors` e `X-Frame-Options` dependem dos headers configurados no Render.
