import { DadosDoUsuario } from "./dados-do-usuario.model";
import { UserToken } from "./userToken";

export class Login {
  validado: boolean;
  userToken: UserToken;
  dadosDoUsuario: DadosDoUsuario;
}