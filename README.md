<!DOCTYPE html>
<html>
<head>
</head>
<body>

<h1>Projeto Atron Tracker</h1>

<p>Esse projeto tem como objetivo exemplificar todo o desenvolvimento e ciclo de um software. O projeto apresentado segue os padrões da Arquitetura Limpa com aplicação parcial do Domain-Driven Design (DDD) e MVC. As bases desse projeto foram desenvolvidas para desktop; no entanto, devido aos conhecimentos adquiridos após exercer minha função na área e dos estudos realizados, refatorei e reescrevi todo o sistema.</p>

<h2>Estrutura dos Projetos</h2>

<ul>
  <li>
    <h3>Framework</h3>
    <ul>
      <li>
        <strong>Communication:</strong> Gerencia a comunicação dentro da aplicação, incluindo serviços de envio de e-mails, notificações, mensagens de texto, ou qualquer outro tipo de comunicação necessária.
      </li>
      <li>
        <strong>ExternalServices:</strong> Responsável pela integração com serviços externos, conectando a aplicação com APIs de terceiros ou serviços como gateways de pagamento, serviços de autenticação, ou APIs de dados.
      </li>
      <li>
        <strong>Notification:</strong> Gerencia notificações dentro da aplicação, incluindo envio de alertas, gerenciamento de status das notificações e definição de regras para disparo de notificações.
      </li>
      <li>
        <strong>Shared:</strong> Contém código compartilhado entre diferentes módulos ou camadas da aplicação, como utilitários, helpers e outras classes reutilizáveis.
      </li>
    </ul>
  </li>

  <li>
    <h3>Atron.Application</h3>
    <p>Contém a lógica de aplicação, como regras de negócios, validações, e manipulação de dados específicos do domínio. Esta camada coordena a execução de tarefas entre diferentes camadas, gerencia casos de uso, e orquestra o funcionamento geral da aplicação.</p>
  </li>

  <li>
    <h3>Atron.Domain</h3>
    <p>Representa a camada de domínio, incluindo entidades principais, regras de negócios essenciais, objetos de valor, e serviços de domínio. Esta camada define os conceitos fundamentais e operações dentro do domínio da aplicação.</p>
  </li>

  <li>
    <h3>Atron.Infra.IoC</h3>
    <p>Responsável pela Injeção de Dependência (IoC - Inversion of Control). Este projeto configura e gerencia a injeção de dependências através de contêineres, permitindo que os diferentes componentes da aplicação sejam desacoplados e testáveis.</p>
  </li>

  <li>
    <h3>Atron.Infrastructure</h3>
    <p>Trata das preocupações de infraestrutura, como o acesso a dados (implementações de repositório), serviços externos, e qualquer outra operação que interaja com o mundo externo ou que suporte as camadas superiores. Contém implementações concretas para interfaces definidas na camada de domínio.</p>
  </li>

  <li>
    <h3>Atron.WebApi</h3>
    <p>A camada de interface da Web para a sua aplicação, expondo APIs RESTful ou endpoints que podem ser consumidos por clientes front-end ou outros serviços. Manipula solicitações HTTP, validações de entrada, autenticação, e retorna respostas apropriadas aos clientes.</p>
  </li>

  <li>
    <h3>Atron.WebViews</h3>
    <p>Focado na interface do usuário, este projeto inclui páginas da web, layouts, e componentes visuais que compõem a interface da sua aplicação. Pode ser um front-end MVC, Razor Pages, ou outra abordagem para renderização de conteúdo dinâmico para os usuários finais.</p>
  </li>
</ul>

</body>
</html>