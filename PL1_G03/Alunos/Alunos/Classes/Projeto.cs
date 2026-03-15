using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Não esta a ser usado, pode ser preciso apgar mais para a frente
namespace Alunos
{
    class Projeto
    {
        public Utilizador Utilizador { get; set; }
        public List<Aluno> Alunos { get; set; } = new();
        public List<Grupo> Grupos { get; set; } = new();
        public List<Tarefa> Tarefas { get; set; } = new();
        public List<Avaliacao> Avaliacoes { get; set; } = new();
        public List<NotaIndividual> NotasIndividuais { get; set; } = new();


    }
}