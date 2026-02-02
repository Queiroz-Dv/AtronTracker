<!DOCTYPE html>
<html>
<head>
</head>
<body>

<h1>Projeto Atron Tracker - Web View (Angular)</h1>

<p>Este é o projeto front-end do sistema Atron Tracker, desenvolvido utilizando Angular. Ele serve como a interface de usuário para interagir com a <a href="[URL_DO_REPOSITÓRIO_DA_API_SE_DISPONÍVEL]">API Atron Tracker</a>, consumindo seus endpoints para exibir e manipular dados.</p>
<p>O projeto visa aplicar conceitos modernos de desenvolvimento front-end, componentização e reatividade, seguindo boas práticas de organização e arquitetura.</p>
<br>
<div>
  <h2>Tecnologias Utilizadas</h2>
  <ul>
    <li><strong><a href="https://angular.io/">Angular</a>:</strong> Framework principal para construção da Single Page Application (SPA).</li>
    <li><strong><a href="https://material.angular.io/">Angular Material</a>:</strong> Biblioteca de componentes UI seguindo as diretrizes do Material Design.</li>
    <li><strong><a href="https://www.typescriptlang.org/">TypeScript</a>:</strong> Superset do JavaScript que adiciona tipagem estática.</li>
    <li><strong><a href="https://rxjs.dev/">RxJS</a>:</strong> Biblioteca para programação reativa usando Observables.</li>
    <li><strong>HTML5 & CSS3:</strong> Estruturação e estilização das páginas.</li>
    <li><strong><a href="https://angular.io/guide/routing-overview">Angular Router</a>:</strong> Para gerenciamento de rotas e navegação.</li>
    <li><strong><a href="https://angular.io/guide/reactive-forms">Reactive Forms</a>:</strong> Para construção de formulários robustos e escaláveis.</li>
    <li><strong><a href="https://angular.io/guide/dependency-injection">Injeção de Dependência (DI)</a>:</strong> Padrão fundamental do Angular para gerenciar dependências.</li>
    <li><strong>Standalone Components:</strong> Arquitetura moderna de componentes no Angular (a partir da v14+).</li>
  </ul>
</div>
<div>
  <h2>Como Configurar?</h2>
  <p>Para configurar e inicializar o sistema front-end, você precisará ter o <a href="https://nodejs.org/">Node.js</a> (que inclui o npm) instalado em sua máquina.</p>
  <ol>
    <li>
      <strong>Clonar o Repositório:</strong>
      <pre><code>git clone [URL_DESTE_REPOSITÓRIO_ANGULAR]</code></pre>
      <pre><code>cd AtronTracker-WebView</code></pre>
    </li>
    <li>
      <strong>Instalar as Dependências:</strong>
      <pre><code>npm install</code></pre>
      <p>Isso instalará todas as bibliotecas listadas no arquivo <code>package.json</code>.</p>
    </li>
    <li>
      <strong>Configurar a API:</strong>
      <p>Certifique-se de que a API Atron Tracker esteja em execução. Configure o endereço da API nos arquivos de ambiente:</p>
      <ul>
        <li><code>src/environments/environment.ts</code> (para produção)</li>
        <li><code>src/environments/environment.development.ts</code> (para desenvolvimento)</li>
      </ul>
      <p>Exemplo (<code>environment.development.ts</code>):</p>
      <pre><code>export const environment = {
  apiRoute: 'https://localhost:44316/', // Endereço da sua API local
  apiLocalRoute: 'https://localhost:44316/',
};</code></pre>
    </li>
    <li>
      <strong>Executar o Servidor de Desenvolvimento:</strong>
      <pre><code>ng serve --open</code></pre>
      <p>Isso compilará o projeto e o servirá localmente (geralmente em <code>http://localhost:4200/</code>), abrindo-o automaticamente no seu navegador padrão.</p>
    </li>
  </ol>
</div>

<div>
  <h2>Como Funciona?</h2>
  <p>O Atron Tracker Web View é uma SPA que se comunica com a Atron Tracker Web API via requisições HTTP para realizar operações CRUD (Create, Read, Update, Delete) nos diversos módulos do sistema (Departamentos, Cargos, Usuários, etc.).</p>
  <ul>
    <li><strong>Autenticação:</strong> Utiliza um sistema de autenticação baseado em tokens (JWT), gerenciado pelo <code>AuthInterceptor</code>, que intercepta as requisições para adicionar o token de autenticação e lidar com a renovação de tokens expirados.</li>
    <li><strong>Navegação:</strong> O Angular Router gerencia as rotas, permitindo a navegação entre as diferentes telas (features) da aplicação. O <code>AuthGuard</code> protege as rotas que exigem autenticação.</li>
    <li><strong>Interface:</strong> A interface é construída com componentes Angular Material, proporcionando uma experiência de usuário consistente e moderna.</li>
    <li><strong>Gerenciamento de Estado:</strong> Embora não utilize uma biblioteca de gerenciamento de estado complexa (como NgRx ou NgXs neste momento), o estado é gerenciado localmente nos componentes e através de serviços compartilhados, utilizando RxJS para lidar com fluxos de dados assíncronos.</li>
    <li><strong>Comunicação com API:</strong> Serviços específicos para cada feature (ex: <code>DepartamentosService</code>, <code>CargoService</code>) encapsulam a lógica de comunicação com a API, herdando funcionalidades de um <code>BaseService</code> ou <code>BaseGenericService</code>.</li>
  </ul>
</div>
<br>

<h2>Estrutura do Projeto Angular</h2>
<p>A estrutura do projeto segue as convenções do Angular CLI, organizada principalmente em módulos de features e pastas core/shared.</p>
<hr>

<ul>
  <li>
    <h3><code>src/app/core</code></h3>
    <p>Contém a lógica central e fundamental da aplicação, que não pertence a nenhuma feature específica.</p>
    <ul>
      <li><strong><code>guards</code>:</strong> Guardas de rota (ex: <code>AuthGuard</code>) para proteger o acesso a certas partes da aplicação.</li>
      <li><strong><code>interceptors</code>:</strong> Interceptadores HTTP (ex: <code>AuthInterceptor</code>) para modificar requisições/respostas (ex: adicionar tokens).</li>
      <li><strong><code>layout</code>:</strong> Componentes de layout reutilizáveis em toda a aplicação (ex: <code>BotaoVoltarComponent</code>, <code>LoadingBarComponent</code>).</li>
      <li><strong><code>services</code>:</strong> Serviços globais da aplicação (ex: <code>BaseService</code>, <code>NotificationService</code>, <code>VisualizacaoService</code>).</li>
      <li><strong><code>validators</code>:</strong> Funções de validação customizadas para Reactive Forms.</li>
    </ul>
  </li>

  <li>
    <h3><code>src/app/features</code></h3>
    <p>Pasta principal contendo os diferentes módulos (ou features) da aplicação. Cada subpasta representa um domínio funcional.</p>
    <ul>
      <li><strong><code>acesso</code>:</strong> Lida com Login e Registro de usuários.</li>
      <li><strong><code>cargos</code>:</strong> CRUD e listagem de Cargos.</li>
      <li><strong><code>dashboard</code>:</strong> Painel inicial com cartões de acesso rápido aos módulos.</li>
      <li><strong><code>departamentos</code>:</strong> CRUD e listagem de Departamentos.</li>
      <li><strong><code>home</code>:</strong> Tela principal com menu lateral após o login.</li>
      <li><strong><code>modulos</code>:</strong> (Pode conter interfaces/modelos relacionados aos módulos do sistema).</li>
      <li><strong><code>perfil-de-acesso</code>:</strong> CRUD e listagem de Perfis de Acesso.</li>
      <li><strong><code>relacionamento-perfil-usuario</code>:</strong> Gerencia a associação entre Usuários e Perfis de Acesso.</li>
      <li><strong><code>salarios</code>:</strong> CRUD e listagem de Salários.</li>
      <li><strong><code>tarefas</code>:</strong> CRUD e listagem de Tarefas.</li>
      <li><strong><code>usuarios</code>:</strong> CRUD e listagem de Usuários.</li>
    </ul>
    <p>Cada pasta de feature geralmente contém:</p>
    <ul>
        <li><strong><code>components</code>:</strong> Componentes específicos da feature (ex: `*-view.component.ts`, `*-edit.component.ts`, `*-form.component.ts`).</li>
        <li><strong><code>models</code>:</strong> Interfaces e classes TypeScript para representar os dados da feature (ex: `*.model.ts`, `request/*.ts`, `response/*.ts`).</li>
        <li><strong><code>services</code>:</strong> Serviços para buscar/manipular dados da API relacionados à feature.</li>
        <li><strong><code>*.module.ts</code> (ou configuração de rotas standalone):</strong> Define as rotas e agrupa os componentes/serviços da feature.</li>
        <li><strong><code>*.component.css</code>:</strong> Estilos específicos da feature.</li>
    </ul>
  </li>

  <li>
    <h3><code>src/app/shared</code></h3>
    <p>Contém componentes, diretivas, pipes, módulos e serviços que são reutilizados em múltiplas features.</p>
    <ul>
      <li><strong><code>components</code>:</strong> Componentes genéricos e reutilizáveis (ex: <code>ConfirmacaoDialogComponent</code>, <code>ControlErrorComponent</code>, <code>UsuarioInformacoesComponent</code>).</li>
      <li><strong><code>models</code>:</strong> Modelos de dados compartilhados (ex: <code>RotasApi</code>).</li>
      <li><strong><code>modules</code>:</strong> Módulos que agrupam exports comuns (ex: <code>SharedModule</code> para módulos do Angular Material, <code>FormModule</code>).</li>
      <li><strong><code>services</code>:</strong> Serviços utilitários compartilhados (ex: <code>ConfirmacaoService</code>, <code>SessaoInfoService</code>).</li>
      <li><strong><code>utils</code>:</strong> Funções utilitárias (ex: <code>formatar-label.util.ts</code>, <code>modulo-functions.util.ts</code>).</li>
    </ul>
  </li>

  <li>
    <h3><code>src/assets</code></h3>
    <p>Arquivos estáticos como imagens, fontes, etc.</p>
  </li>

  <li>
    <h3><code>src/environments</code></h3>
    <p>Arquivos de configuração específicos para cada ambiente (desenvolvimento, produção, etc.), principalmente para definir a URL da API.</p>
  </li>

  <li>
    <h3>Arquivos Raiz (<code>src/</code>)</h3>
    <ul>
      <li><strong><code>index.html</code>:</strong> O arquivo HTML principal.</li>
      <li><strong><code>main.ts</code>:</strong> Ponto de entrada da aplicação, onde o Angular é inicializado.</li>
      <li><strong><code>main.css</code>:</strong> Estilos globais da aplicação.</li>
    </ul>
  </li>

   <li>
    <h3>Arquivos Raiz (Projeto)</h3>
    <ul>
      <li><strong><code>angular.json</code>:</strong> Arquivo de configuração do Angular CLI (build, serve, test, etc.).</li>
      <li><strong><code>package.json</code>:</strong> Lista as dependências do projeto e scripts npm.</li>
      <li><strong><code>tsconfig.*.json</code>:</strong> Arquivos de configuração do TypeScript.</li>
    </ul>
  </li>
</ul>
</body>
</html>
