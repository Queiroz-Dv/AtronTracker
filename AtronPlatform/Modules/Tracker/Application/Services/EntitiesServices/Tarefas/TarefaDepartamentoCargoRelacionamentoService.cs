using Application.DTO;
using Application.Resources;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaDepartamentoCargoRelacionamentoService(
        IDepartamentoRepository departamentoRepository,
        ICargoRepository cargoRepository)
    {
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly ICargoRepository _cargoRepository = cargoRepository;

        public async Task<Resultado<Departamento>> RelacionarAsync(Tarefa tarefa, TarefaDTO tarefaDTO)
        {
            if (tarefaDTO.DepartamentoCodigo.IsNullOrEmpty())
            {
                tarefa.RemoverDepartamento();
                return Resultado<Departamento>.Sucesso(null);
            }

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(tarefaDTO.DepartamentoCodigo.ToUpper());
            if (departamento is null)
                return Resultado<Departamento>.Falha(TarefaResource.Erro_DepartamentoNaoEncontrado);

            tarefa.VincularDepartamento(departamento);            

            if (tarefaDTO.CargoCodigo.IsNullOrEmpty())
            {
                tarefa.RemoverCargo();
                return Resultado<Departamento>.Sucesso(departamento);
            }

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(tarefaDTO.CargoCodigo.ToUpper());
            if (cargo is null)
                return Resultado<Departamento>.Falha(TarefaResource.Erro_CargoNaoEncontrado);

            if (cargo.DepartamentoId != departamento.Id ||
                cargo.DepartamentoCodigo != departamento.Codigo)

                return Resultado<Departamento>.Falha(TarefaResource.Erro_CargoNaoPertenceDepartamento);

            tarefa.VincularCargo(cargo);            

            return Resultado<Departamento>.Sucesso(departamento);
        }        
    }
}