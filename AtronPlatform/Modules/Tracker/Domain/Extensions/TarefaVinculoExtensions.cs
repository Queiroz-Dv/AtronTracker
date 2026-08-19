using Domain.Entities;

namespace Domain.Extensions
{
    public static class TarefaVinculoExtensions
    {
        public static void VincularDepartamento(this Tarefa tarefa, Departamento departamento)
        {
            tarefa.Departamento = departamento;
            tarefa.DepartamentoId = departamento.Id;
            tarefa.DepartamentoCodigo = departamento.Codigo;
        }

        public static void VincularCargo(this Tarefa tarefa, Cargo cargo)
        {
            tarefa.Cargo = cargo;
            tarefa.CargoId = cargo.Id;
            tarefa.CargoCodigo = cargo.Codigo;
        }

        public static void VincularUsuario(this Tarefa tarefa,int usuarioId, string usuarioCodigo, Usuario usuario = null)
        {
            tarefa.Usuario = usuario;
            tarefa.UsuarioId = usuarioId;
            tarefa.UsuarioCodigo = usuarioCodigo;
        }   
    }
}