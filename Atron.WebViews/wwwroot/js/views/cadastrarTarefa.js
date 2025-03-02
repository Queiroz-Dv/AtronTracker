﻿class CadastrarTarefa {
    constructor(tarefaId, usuarioSelectId, tarefaFormContainerId, actionPage) {
        this.usuarioSelect = document.getElementById(usuarioSelectId);
        this.tarefaIdentity = document.getElementById(tarefaId);
        this.tarefaFormContainer = document.getElementById(tarefaFormContainerId);
        this.formAction = document.getElementById(actionPage);
        this.init();
    }

    init() {
        this.usuarioSelect.addEventListener('change', () => this.carregarFormulario());
    }

    async carregarFormulario() {
        const codigoUsuario = this.usuarioSelect.value;
        const actionPage = this.formAction.value;
        const tarefaId = this.tarefaIdentity.value;

        // Limpa o formulário caso nenhum usuário seja selecionado
        if (!codigoUsuario) {      
            return;
        }

        try {
            const response = await fetch(`/Tarefa/CarregarFormularioTarefa?tarefaId=${tarefaId}&codigoUsuario=${codigoUsuario}&actionPage=${actionPage}`);

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
    new CadastrarTarefa('tarefaId', 'usuarioCodigo', 'tarefaFormContainer', 'actionPage');
});
