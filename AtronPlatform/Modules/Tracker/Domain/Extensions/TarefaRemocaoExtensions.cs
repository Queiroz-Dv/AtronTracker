using Domain.Entities;

namespace Domain.Extensions
{
    public static class TarefaRemocaoExtensions
    {
        public static void RemoverDepartamento(this Tarefa tarefa)
        {
            tarefa.Departamento = null;
            tarefa.DepartamentoId = null;
            tarefa.DepartamentoCodigo = null;
        }

        public static void RemoverCargo(this Tarefa tarefa)
        {
            tarefa.Cargo = null;
            tarefa.CargoId = null;
            tarefa.CargoCodigo = null;
        }

        public static void RemoverUsuario(this Tarefa tarefa)
        {
            tarefa.Usuario = null;
            tarefa.UsuarioId = null;
            tarefa.UsuarioCodigo = null;
        }
    }
}