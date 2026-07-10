using Domain.Entities;
using System.Collections.Generic;

namespace Application.Specifications.UsuarioSpecifications
{
    public class UsuarioSpecification : ISpecification<Usuario>
    {
        private readonly string _codigoUsuario;
        //private readonly string _nome;
        //private readonly string _sobrenome;
        private readonly string _email;

        public UsuarioSpecification(
            string codigoUsuario,
            //string nome,
            //string sobrenome,
            string email)
        {
            _codigoUsuario = codigoUsuario;
            //_nome = nome;
            //_sobrenome = sobrenome;
            _email = email;
        }

        public List<string> Errors { get; private set; } = new List<string>();

        public bool IsSatisfiedBy(Usuario entity)
        {
            if (!entity.Codigo.Equals(_codigoUsuario))
                Errors.Add("O código do usuário não corresponde.");

            //if (!entity.Nome.Equals(_nome))
            //    Errors.Add("O nome do usuário não corresponde.");

            //if(!entity.Sobrenome.Equals(_sobrenome))
            //    Errors.Add("O sobrenome do usuário não corresponde.");

            if (!entity.Email.Equals(_email))
                Errors.Add("O email do usuário não corresponde.");

            return Errors.Count == 0;
        }
    }
}