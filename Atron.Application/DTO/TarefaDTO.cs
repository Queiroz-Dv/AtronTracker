using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class TarefaDTO
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int UsuarioId { get; set; }

        [MaxLength(10, ErrorMessage = "O tamanho máximo do código de usuário é de 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código de cargo é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo código do cargo é obrigatório.")]
        public string UsuarioCodigo { get; set; }

        [ValidateNever][JsonIgnore] public string NomeUsuario { get; set; }
        [ValidateNever][JsonIgnore] public string CargoDescricao { get; set; }
        [ValidateNever][JsonIgnore] public string DepartamentoDescricao { get; set; }

        [DisplayName("Título")]
        [MaxLength(50, ErrorMessage = "O tamanho máximo para o título é de 50 caracteres.")]
        [MinLength(5, ErrorMessage = "O tamanho mínimo para o título é de 5 caracteres.")]
        [Required(ErrorMessage = "O campo título é obrigatório.")]
        public string Titulo { get; set; }

        [DisplayName("Conteúdo")]
        [MaxLength(2500, ErrorMessage = "O tamanho máximo para o conteúdo é de 2500 caracteres.")]
        public string Conteudo { get; set; }

        [DisplayName("Data inicial")]
        public DateTime DataInicial { get; set; }

        [DisplayName("Data final")]
        public DateTime DataFinal { get; set; }

        [DisplayName("Estado da tarefa")]
        [Required(ErrorMessage = "Necessário ter um estado da tarefa.")]
        public string EstadoDaTarefaId { get; set; }

        [DisplayName("Estado")]
        [JsonIgnore]
        public string EstadoDaTarefaDescricao { get; set; }
    }
}