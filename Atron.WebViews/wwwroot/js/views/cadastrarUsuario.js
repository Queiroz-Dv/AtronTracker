import { Departamento } from "../models/departamentoModel.js";
import { Cargo } from "../models/cargoModel.js";

class CadastrarPage {
    constructor(departamentoSelectId, cargoSelectId) {
        this.departamentoSelect = document.getElementById(departamentoSelectId);
        this.cargoSelect = document.getElementById(cargoSelectId);

        this.init();
    }

    // Inicializa os eventos
    init() {
        this.departamentoSelect.addEventListener('change', () => this.carregarCargos());
        this.cargoSelect.addEventListener('change', () => this.carregarDepartamentos());
    }

    // Faz uma requisição para obter os cargos associados
    async carregarCargos() {
        const codigoDepartamento = this.departamentoSelect.value;

        try {

            const response = await fetch(`/Usuario/ObterCargosAssociados?codigoDepartamento=${codigoDepartamento}`);
            const cargosData = await response.json();

            const cargos = cargosData.map(cargo => new Cargo(cargo.codigo, cargo.descricao));

            this.cargoSelect.innerHTML = '<option value="">Selecione um cargo</option>'; // Reseta as opções

            cargos.forEach(cargo => {
                const option = document.createElement('option');
                option.value = cargo.codigo;
                option.textContent = cargo.descricao;
                this.cargoSelect.appendChild(option);
            });
        } catch (error) {
            console.error('Erro ao carregar cargos:', error);
        }
    }

    async carregarDepartamentos() {
        const codigoCargo = this.cargoSelect.value;
        const codigoDepartamento = this.departamentoSelect.value;

        if (codigoDepartamento) {
            return;
        } else {

            try {
                const response = await fetch(`/Usuario/ObterDepartamentosAssociados?codigoCargo=${codigoCargo}`);
                const departamentosData = await response.json();

                const departamentos = departamentosData.map(departamento => new Departamento(departamento.codigo, departamento.descricao));

                this.departamentoSelect.innerHTML = '<option value="">Selecione um departamento</option>'; // Reseta as opções

                departamentos.forEach(departamento => {
                    const option = document.createElement('option');
                    option.value = departamento.codigo;
                    option.textContent = departamento.descricao;
                    this.departamentoSelect.appendChild(option);
                });

            } catch (error) {
                console.error('Erro ao carregar cargos:', error);
            }
        }
    }
}

// Instancia a classe quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', () => {
    new CadastrarPage('departamentoCodigo', 'cargoCodigo');
});
