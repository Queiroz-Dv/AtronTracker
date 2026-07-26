# Inventário de registro e acesso de usuário

## Escopo protegido

`RegistroUsuarioService` concentra quatro casos de uso públicos: cadastro,
confirmação de e-mail, solicitação de recuperação e troca de senha. Os
contratos HTTP permanecem em `AcessoController` e retornam `Resultado`.

## Efeitos por fluxo

| Fluxo | Efeitos e colaboradores |
| --- | --- |
| Cadastro | Validador, Identity, usuário de negócio, perfil opcional, confirmação de e-mail e aviso de envio. |
| Confirmação | Normalização do código, confirmação ativa, marcação de usuário e confirmação concluída por e-mail como aviso. |
| Recuperação | Busca normalizada por código ou e-mail, token da Identity, dados temporários no cache e e-mail obrigatório. |
| Troca de senha | Dados temporários, descriptografia, Identity, login e remoção do cache após sucesso. |

## Regras preservadas

- a normalização de código ocorre no limite da aplicação antes da chamada aos
  repositórios;
- falha no envio do e-mail de cadastro ou de confirmação concluída é aviso;
- falha no e-mail obrigatório de recuperação impede o sucesso;
- a troca de senha só remove o dado temporário após a redefinição pela Identity.

Os testes em `Application.Tests/AuthServices/RegistroUsuarioServiceTests.cs`
protegem a seleção por código ou e-mail, a normalização, a criação da
confirmação e o caráter consultivo do e-mail de cadastro.
