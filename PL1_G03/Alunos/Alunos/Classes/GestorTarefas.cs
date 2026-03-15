using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Alunos
{
    public static class GestorTarefas
    {
        // Importar tarefas de CSV para a lista global
        public static void ImportarTarefasDeCSV(string caminhoCSV)
        {
            var app = (App)System.Windows.Application.Current;
            var novaLista = new ObservableCollection<Tarefa>();

            foreach (var linha in File.ReadAllLines(caminhoCSV, Encoding.UTF8).Skip(1)) // Ignora cabeçalho
            {
                var partes = linha.Split(',');
                if (partes.Length >= 6 &&
                    double.TryParse(partes[2], out double peso) &&
                    DateTime.TryParse(partes[3], out DateTime dataInicio) &&
                    DateTime.TryParse(partes[4], out DateTime dataTermino))
                {
                    novaLista.Add(new Tarefa
                    {
                        Titulo = partes[0],
                        Descricao = partes[1],
                        Peso = peso,
                        DataInicio = dataInicio,
                        DataTermino = dataTermino,
                        TipoTarefa = partes[5]
                    });
                }
            }

            app.Tarefas.Clear();
            foreach (var tarefa in novaLista)
                app.Tarefas.Add(tarefa);

            app.SaveTarefas();
        }

        // Exportar tarefas para CSV
        public static void ExportarTarefasParaCSV(string caminhoCSV)
        {
            var app = (App)System.Windows.Application.Current;
            var linhas = new[]
            {
                "Titulo,Descricao,Peso,DataInicio,DataTermino,TipoTarefa"
            }.Concat(app.Tarefas.Select(t =>
                $"{t.Titulo},{t.Descricao},{t.Peso},{t.DataInicio:yyyy-MM-dd HH:mm},{t.DataTermino:yyyy-MM-dd HH:mm},{t.TipoTarefa}"));

            File.WriteAllLines(caminhoCSV, linhas, Encoding.UTF8);
        }
    }
}