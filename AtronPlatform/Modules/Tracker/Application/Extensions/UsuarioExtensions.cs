using Application.DTO.Request;
using Domain.Entities;
using System;

namespace Application.Extensions
{
    public static class UsuarioExtensions
    {
        public static Usuario MapearRequestParaEntidade(this UsuarioRegistroRequest request)
        {
            return new Usuario()
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                Sobrenome = request.Sobrenome,
                DataNascimento = request.DataNascimento.HasValue ? request.DataNascimento.Value.ToDateTime(new TimeOnly()) : null,
                Email = request.Email,
            };
        }
    }
}    
