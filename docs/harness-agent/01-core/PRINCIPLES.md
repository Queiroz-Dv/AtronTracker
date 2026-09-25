# Princípios de Engenharia do Atron

## 1. Entender antes de modificar

Código existente é evidência. Não substitua uma implementação sem entender por que ela existe. Use o git para avaliar o histório se necessário.

## 2. Simplicidade antes de abstração

Uma abstração deve reduzir complexidade total, não apenas deslocá-la.

## 3. Modularidade sem fragmentação

Módulos devem possuir responsabilidades claras. Modularidade não significa criar dezenas de projetos, interfaces ou serviços artificiais.

## 4. Domínio primeiro

Regras de negócio pertencem ao domínio/aplicação adequada. Controllers e componentes não devem se tornar depósitos de regras.

## 5. Segurança por padrão

Falhas de autorização, isolamento de tenant e exposição de dados são problemas críticos.

## 6. Evidência antes de otimização

Não introduza cache, filas, microservices ou complexidade operacional apenas por antecipação.

## 7. Mudanças pequenas

Uma alteração pequena e verificável é preferível a uma grande refatoração não solicitada.

## 8. Compatibilidade

Ao modificar contratos, considere consumidores, migrations, APIs e frontend.

## 9. Observabilidade

Quando uma operação importante falhar, preserve informação suficiente para diagnosticar o problema sem expor dados sensíveis.

## 10. Não confundir padrão com solução

Clean Architecture, DDD, CQRS, MediatR, repository, facade, process manager, microservices e outros padrões são ferramentas. Nenhum deles deve ser aplicado apenas por prestígio técnico.
