# Arquitetura dos testes automatizados

## Objetivo

Os testes automatizados ficam centralizados em `Tests` na raiz da solução. A organização facilita localizar a cobertura de cada módulo sem transformar `Framework` em uma pasta genérica de itens que não pertencem ao código compartilhado de produção.

## Convenção de nomes

Cada projeto de teste usa o nome curto do sistema ou módulo, sem repetir sua camada interna:

| Módulo ou sistema | Projeto de teste |
| --- | --- |
| Atron Tracker | `Tracker.Tests` |
| Atron Stock | `Stock.Tests` |
| Framework Shared | `Shared.Tests` |
| Atron Notificacoes | `Notificacoes.Tests` |

O nome do projeto representa o dono da funcionalidade. `Application`, `Domain`, `Infrastructure` e `Client` são detalhes de referência e podem aparecer apenas em pastas internas quando ajudarem a localizar o teste.

## Estrutura

```text
Tests/
  Tracker.Tests/
    Tarefas/
    Resources/
    Acesso/
  Stock.Tests/
    Estoque/
  Shared.Tests/
    Email/
    Resources/
  Notificacoes.Tests/
    Contratos/
    Cliente/
    Integracao/
    Autorizacao/
```

Uma funcionalidade recebe pasta, não um novo projeto, quando compartilha ciclo de execução, pacotes e fronteira de integração com o restante do módulo. Um novo projeto só é justificado quando há uma diferença real de processo hospedado, banco ou infraestrutura externa, dependência de pacote incompatível, tempo de execução ou ciclo de publicação.

## Regras de dependência

- Projetos de teste podem referenciar projetos de produção; projetos de produção nunca referenciam testes.
- Um arquivo de teste pertence a apenas um projeto. Não usar `Compile Include` com `Link` para executar o mesmo fonte em mais de um assembly.
- Testes de contrato, cliente HTTP e integração da central começam em `Notificacoes.Tests`, separados por pasta. A separação em novos projetos requer evidência de necessidade operacional.
- Testes que verificam uma regra de produto ficam no módulo produtor. Por exemplo, a criação de uma tarefa continua testada em `Tracker.Tests`, embora publique pela central.

## Migração incremental

1. Capturar a lista de testes dos projetos atuais e registrar a linha de base; no Tracker, identificar explicitamente o teste vinculado que hoje é compilado por dois projetos.
2. Criar a pasta de solução `Tests` e mover `Tracker.Tests` para ela.
3. Unificar os testes atuais de `Application.Tests` e `Application.Resources.Tests`, removendo o arquivo vinculado de tarefa.
4. Executar `dotnet test` do novo projeto e da solução inteira.
5. Repetir para Stock, Shared e Notificacoes, uma fatia por vez.
6. Atualizar filtros de CI somente depois de cada projeto passar com o novo caminho e nome.

## Validação

Cada migração de projeto deve preservar a contagem de testes, `git diff --check`, execução do projeto movido e `dotnet test AtronPlatform.sln` com saída isolada quando houver DLLs bloqueadas.
