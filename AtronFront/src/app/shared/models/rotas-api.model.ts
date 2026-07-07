import { environment } from "../../../environments/environment.development";

export class RotasApi {
  public static readonly logarEndpoint: string = environment.apiRoute + 'api/Acesso/Login';
  public static readonly registrarEndpoint: string = environment.apiRoute + 'api/Acesso/Registrar';
  public static readonly confirmarEmailEndpoint: string = environment.apiRoute + 'api/Acesso/ConfirmarEmail';
  public static readonly desconectarEndpoint: string = environment.apiRoute + 'api/Acesso/Desconectar';
  public static readonly refreshTokenEndpoint: string = environment.apiRoute + 'api/Acesso/RefreshToken';
  public static readonly sessionInfoEndpoint: string = environment.apiRoute + 'api/Sessao/Info';

  public static readonly recuperarSenhaEndpoint: string = environment.apiRoute + 'api/Acesso/RecuperarSenha';
  public static readonly trocarSenhaEndpoint: string = environment.apiRoute + 'api/Acesso/TrocarSenha';

  // Padrão comum dos endpoints das rotas de cada módulo
  public static readonly departamentoEndpoint: string = environment.apiRoute + 'api/Departamento';
  public static readonly cargoEndpoint: string = environment.apiRoute + 'api/Cargo';
  public static readonly usuarioEndpoint: string = environment.apiRoute + 'api/Usuario';
  public static readonly tarefaEndpoint: string = environment.apiRoute + 'api/Tarefa';
  public static readonly tarefaEstadosEndpoint: string = environment.apiRoute + 'api/Tarefa/Estados';
  public static readonly notificacaoInternaEndpoint: string = environment.apiRoute + 'api/NotificacaoInterna';
  public static readonly planejamentoCustoEndpoint: string = environment.apiRoute + 'api/PlanejamentoCusto';

  // Politicas e Acesso
  public static readonly moduloEndpoint: string = environment.apiRoute + 'api/Modulo';
  public static readonly perfilDeAcessoEndpoint: string = environment.apiRoute + 'api/PerfilDeAcesso';
  public static readonly obter_relacionamentoPerfilUsuarioEndpoint: string = environment.apiRoute + 'api/PerfilDeAcesso/ObterRelacionamentoPerfilUsuario'
  public static readonly gravar_relacionamentoPerfilUsuarioEndpoint: string = environment.apiRoute + 'api/PerfilDeAcesso/RelacionamentoPerfilUsuario'
}
