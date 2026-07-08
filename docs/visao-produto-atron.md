# Visão de produto do Atron

## Propósito

O Atron é uma plataforma comercial simples para negócios locais e pequenos empresários que desejam centralizar a gestão da empresa sem assumir custo inicial alto de tecnologia.

O objetivo do produto é entregar uma base funcional, organizada e evolutiva, com tecnologia suficiente para apoiar fluxos reais de administração, operação, suprimentos, patrimônio, vendas e financeiro. O produto não busca competir por excesso de complexidade. A direção é cobrir rotinas importantes com clareza, baixo custo operacional e arquitetura que permita manutenção contínua.

## Publico-alvo

O Atron deve atender principalmente:

- negócios locais que precisam sair de controles dispersos;
- pequenos empresários que precisam centralizar cadastros, tarefas, estoque, suprimentos e vendas;
- operações pequenas que precisam de rastreabilidade sem implantar um ERP caro;
- ambientes onde simplicidade, custo reduzido e manutenção previsível importam mais do que customização pesada.

## Posicionamento

O Atron deve ser tratado como uma plataforma de gestão comercial de porte enxuto. Ele precisa ser simples o suficiente para uma pequena empresa conseguir rodar normalmente, mas estruturado o suficiente para que os módulos cresçam sem virar uma coleção de telas soltas.

Princípios do produto:

- custo inicial reduzido;
- uso prático por negócios pequenos;
- regras de negócio no backend;
- front principal em Angular;
- documentação como contrato de domínio;
- evolução modular;
- manutenção e correção como parte do valor do produto.

## Módulos de produto

### Atron Tracker

O Atron Tracker concentra os fluxos internos de gestão, acompanhamento e estrutura organizacional.

Ele deve cobrir rotinas como:

- usuários;
- departamentos;
- cargos;
- perfis de acesso;
- relacionamento entre usuário e perfil;
- tarefas;
- notificações internas;
- planejamento de custos;
- outras rotinas administrativas que organizam o funcionamento interno da empresa.

O Tracker é a base de governança operacional do Atron. Ele organiza quem existe no sistema, quais responsabilidades existem, quais áreas compõem a empresa e como tarefas e planejamentos são acompanhados.

### Atron Stock

O Atron Stock é o módulo destinado a lidar com a cadeia de suprimentos, estoque, patrimônio e bens da empresa.

Ele deve evoluir para cobrir:

- produtos;
- categorias;
- fornecedores;
- clientes quando relacionados ao fluxo de estoque e venda;
- entradas;
- saídas;
- movimentações;
- saldos;
- rastreabilidade;
- controle de bens e patrimônio;
- rotinas de suprimentos.

O Stock deve proteger a consistência das movimentações e oferecer uma cobertura ampla das rotinas físicas e patrimoniais da empresa, sem depender de controle manual externo como fonte principal.

### Atron Sales

O Atron Sales é o módulo planejado para concentrar o comercial e o financeiro da empresa.

Ele deve ser modelado com cuidado porque tende a conectar venda, recebimento, faturamento, contas, fluxo de caixa e informações que podem afetar decisões financeiras. Enquanto estiver em planejamento, a documentação deve separar claramente o que já existe no sistema daquilo que ainda é direção futura.

Escopos candidatos para o Sales:

- vendas;
- propostas;
- clientes comerciais;
- recebimentos;
- contas a receber;
- contas a pagar;
- formas de pagamento;
- visão financeira básica;
- relatórios comerciais e financeiros.

## Direção arquitetural

O Atron deve preservar uma separação clara entre produto, regra de negócio e apresentação.

Diretrizes:

- regras de negócio e mensagens finais de validação devem nascer no backend;
- o Angular deve cuidar de interação, estado visual, validações de formato e consumo das APIs;
- cada módulo deve ter linguagem própria documentada antes de crescer em funcionalidades críticas;
- decisões grandes de domínio devem ser registradas em ADR;
- documentação de produto deve diferenciar escopo atual, escopo planejado e ideias futuras;
- o sistema deve favorecer manutenção previsível em vez de atalhos que aumentem custo de correção.

## Objetivo pessoal do projeto

O Atron também existe como uma trilha de formação prática em arquitetura de software.

O objetivo pessoal é provar, por meio de um produto real, que é possível escalar, manter, corrigir e documentar um software com qualidade, compromisso e critério técnico. A evolução do Atron deve, portanto, servir a dois resultados ao mesmo tempo:

- entregar uma plataforma comercial funcional para negócios pequenos;
- formar repertório real de arquitetura, produto, manutenção, decisão técnica e evolução modular.

## Contrato de documentação

A documentação do Atron deve ser mantida como parte ativa do produto.

Cada nova fase deve responder:

- qual problema de negócio está sendo resolvido;
- qual módulo do produto é responsável pelo fluxo;
- quais termos passam a ser canônicos;
- quais regras pertencem ao backend;
- quais telas ou APIs representam o fluxo;
- quais limites ficam fora do escopo da fase;
- qual decisão precisa virar ADR.

Esse contrato existe para impedir que o Atron cresça apenas por acumulação de código. O produto deve crescer por domínio entendido, regra registrada e implementação alinhada.
