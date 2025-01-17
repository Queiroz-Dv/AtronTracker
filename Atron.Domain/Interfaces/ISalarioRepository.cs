﻿using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ISalarioRepository : IRepository<Salario>
    {
        Task CriarSalarioAsync(Salario entidade);
        Task<Salario> ObterSalarioPorCodigoUsuario(string codigoUsuario);

        Task AtualizarSalarioRepositoryAsync(Salario salario);
        Task<Salario> ObterSalarioPorIdAsync(int id);

        Task<List<Salario>> ObterSalariosRepository();
    }
}