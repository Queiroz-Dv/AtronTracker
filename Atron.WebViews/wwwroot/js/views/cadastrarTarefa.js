class CadastrarTarefa {
    constructor(usuarioSelectId, tarefaFormContainerId) {
        this.usuarioSelect = document.getElementById(usuarioSelectId);
        this.tarefaFormContainer = document.getElementById(tarefaFormContainerId);

        this.init();
    }

    init() {
        this.usuarioSelect.addEventListener('change', () => this.carregarFormulario());
    }

    async carregarFormulario() {
        const codigoUsuario = this.usuarioSelect.value;

        // Limpa o formulário caso nenhum usuário seja selecionado
        if (!codigoUsuario) {
            this.tarefaFormContainer.innerHTML = "";
            return;
        }

        try {
            const response = await fetch(`/Tarefa/CarregarFormularioTarefa?codigoUsuario=${codigoUsuario}`);

            if (response.ok) {
                const html = await response.text();
                this.tarefaFormContainer.innerHTML = html; // Insere o formulário no container
            } else {
                this.tarefaFormContainer.innerHTML = `<p class="text-danger">Erro ao carregar o formulário: ${response.statusText}</p>`;
            }
        } catch (error) {
            this.tarefaFormContainer.innerHTML = `<p class="text-danger">Ocorreu um erro: ${error.message}</p>`;
        }
    }
}

// Inicializa o script
document.addEventListener('DOMContentLoaded', () => {
    new CadastrarTarefa('usuarioCodigo', 'tarefaFormContainer');
});














//import { Tarefa } from "../models/tarefaModel.js";

//class CadastrarTarefa {
//    constructor(usuarioSelectId) {
//        this.usuarioSelect = document.getElementById(usuarioSelectId);
//        this.init();
//    }

//    init() {
//        this.usuarioSelect.addEventListener('change', () => this.carregarTarefaForm());
//    }


//    async carregarTarefaForm() {
//        const codigoUsuario = this.usuarioSelect.value;

//        try {
//            const response = await fetch(`/Usuario/ObterUsuarioPorCodigo?codigoUsuario=${codigoUsuario}`);

//            const usuarioData = await response.json();

//            const tarefaData = new Tarefa(usuarioData.codigo, usuarioData.cargoDescricao, usuarioData.departamentoDescricao);
            
//        } catch (error) {

//        }
//    }
//}

//document.addEventListener('DOMContentLoaded', () => {
//    new CadastrarTarefa('usuarioCodigo');
//});