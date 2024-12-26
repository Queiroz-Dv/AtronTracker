class TarefaForm {
	constructor(dataInicialId, dataFinalId, formId) {
		this.dataInicialInput = document.getElementById(dataInicialId);
		this.dataFinalInput = document.getElementById(dataFinalId);
		this.formContent = document.getElementById(formId);

		this.init();
	}

	init() {
		this.formContent.addEventListener('load', () => this.carregarDatas());
	}

	async carregarDatas() {
		const dataInicial = this.dataInicialInput.value;
		const dataFinal = this.dataFinalInput.value;

		console.log(dataInicial)
	}
}

document.addEventListener('DOMContentLoaded', () => {
	new TarefaForm('dataInicial', 'dataFinal', 'formTarefa');
});