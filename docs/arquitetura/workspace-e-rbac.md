# Documentação entre Workspace e RBAC

O sistema será modificado para adotar a abordagem de tenant mantendo a mesma base de dados.

Todos os registros passarão a incluir a chave do workspace cadastrado.
O RBAC existente permanecerá como modelo de negócio o workspace servirá apenas como limite para os tenants como citado anteriormente.

Atualmente a estrutura de login e sessão consideram apenas perfil de acesso e módulos, mas com a nova correção
passaremos a incluir o workspace para identificar a sessão do usuário e os acessos dele de acordo com o workspace registrado.