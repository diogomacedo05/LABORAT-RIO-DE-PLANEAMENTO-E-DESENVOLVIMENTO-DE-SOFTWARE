using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Alunos
{
    public class LinhaPauta : INotifyPropertyChanged
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public Dictionary<string, double> NotasPorTarefa { get; set; } = new();
        public double AvaliacaoFinal { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nome) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }

    public class Pautas
    {
        public ObservableCollection<LinhaPauta> Linhas { get; set; }
        public List<string> Tarefas { get; set; }

        public Pautas(ObservableCollection<Aluno> alunos, ObservableCollection<Tarefa> tarefas, App app)
        {
            // Lista de títulos das tarefas
            Tarefas = app.Tarefas.Select(t => t.Titulo).ToList();

            // Gera a matriz de alunos x tarefas
            Linhas = new ObservableCollection<LinhaPauta>(
                app.Alunos.Select(aluno =>
                {
                    var notasPorTarefa = new Dictionary<string, double>();
                    foreach (var tarefa in Tarefas)
                    {
                        var nota = app.NotasIndividuais
                            .FirstOrDefault(n => n.NumeroAluno == aluno.Numero && n.TituloTarefa == tarefa)?.Nota ?? 0.0;
                        notasPorTarefa[tarefa] = nota;
                    }
                    // Avaliação final: média das notas (podes adaptar para ponderação)
                    double avaliacaoFinal = notasPorTarefa.Values.Any() ? notasPorTarefa.Values.Average() : 0.0;
                    return new LinhaPauta
                    {
                        Numero = aluno.Numero,
                        Nome = aluno.Nome,
                        NotasPorTarefa = notasPorTarefa,
                        AvaliacaoFinal = avaliacaoFinal
                    };
                })
            );
        }
    }
}
