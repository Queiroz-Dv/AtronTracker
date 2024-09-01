## Projeto Infra.IoC

O projeto `Atron.Infra.IoC` é a camada de conteiner para tratar as dependências do sistema. <br>
Nesse projeto teremos apenas as interfaces e classes que realizam suas devidas implementações,
desacoplando a iniciliazação das implementações concretamente via construtor.

## Componentes Principais

### Dependency Injection

Essa classe é utilizada para os processos da View, ou seja, ela não tem liberdade para acessar os processos do banco de dados. <br>

### Dependency Injection Api

Essa classe é utilizada pela API onde irá conter todos os processos, configurações e fluxos do sistema.