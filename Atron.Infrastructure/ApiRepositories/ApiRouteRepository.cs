﻿using Atron.Domain.ApiEntities;
using Atron.Domain.ApiInterfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.ApiRepositories
{
    public class ApiRouteRepository : IApiRouteRepository
    {
        private AtronDbContext _context;

        public ApiRouteRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task CriarRotaRepositoryAsync(ApiRoute apiRoute)
        {
            await _context.ApiRoutes.AddAsync(apiRoute);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<IEnumerable<ApiRoute>> ObterTodasRotas()
        {
            var rotas = await _context.ApiRoutes.Where(rts => rts.Ativo).ToListAsync();
            return rotas;
        }
    }
}