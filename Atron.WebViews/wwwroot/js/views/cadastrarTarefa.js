import { Tarefa } from "../models/tarefaModel.js";

class CadastrarTarefa {
    constructor(usuarioSelectId) {
        this.usuarioSelect = document.getElementById(usuarioSelectId);
        this.init();
    }
    init() {
        this.usuarioSelect.addEventListener('change', () => this.carregarTarefaForm());
    }


    async carregarTarefaForm() {
        const codigoUsuario = this.usuarioSelect.value;

        try {
            const response = await fetch(`/Usuario/ObterUsuarioPorCodigo?codigoUsuario=${codigoUsuario}`);

            const usuarioData = await response.json();

            const tarefaData = usuarioData.map(usr => new Tarefa(usr.codigo, usr.cargoDescricao, usr.departamentoDescricao));

            const div = document.createElement('div');

            div.innerHTML = "<fieldSet><legend>Info. do usuário</legend></fieldSet>"

            
            console.log(div);

        } catch (error) {

        }
    }
}