﻿using System;

namespace Atron.Application.DTO.ApiDTO
{
    public class RegisterDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string ConfirmaSenha { get; set; }
    }
}