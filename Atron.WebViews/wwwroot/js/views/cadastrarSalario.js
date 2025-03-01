﻿class CadastrarSalario {
    constructor(salarioId, usuarioSelectId, salarioFormContainerId, actionPage) {
        this.salarioIdentity = document.getElementById(salarioId);
        this.usuarioSelect = document.getElementById(usuarioSelectId);
        this.tarefaFormContainer = document.getElementById(salarioFormContainerId);
        this.formAction = document.getElementById(actionPage);
        this.init();
    }

    init() {
        this.usuarioSelect.addEventListener('change', () => this.carregarFormulario());
    }

    async carregarFormulario() {
        const codigoUsuario = this.usuarioSelect.value;
        const actionPage = this.formAction.value;
        let salarioId = 0;

        if (this.salarioIdentity != null) {
             salarioId = this.salarioIdentity.value;
        }

        // Limpa o formulário caso nenhum usuário seja selecionado
        if (!codigoUsuario) {
            return;
        }

        try {           
            const response = await fetch(`/Salario/CarregarFormularioSalario?salarioId=${salarioId}&codigoUsuario=${codigoUsuario}&actionPage=${actionPage}`);

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
    new CadastrarSalario('salarioId', 'usuarioCodigo', 'salarioFormContainer', 'actionPage');
});
