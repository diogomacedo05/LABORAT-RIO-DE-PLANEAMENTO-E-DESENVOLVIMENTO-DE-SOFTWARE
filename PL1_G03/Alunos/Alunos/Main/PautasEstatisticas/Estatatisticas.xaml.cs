using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace Alunos
{
    public partial class Estatatisticas : Window
    {
        public Estatatisticas()
        {
            InitializeComponent();
            DataContext = Application.Current as App;
            AtualizarAvaliacoes();
        }

        private void AtualizarAvaliacoes()
        {
            // Garante que os dados estão atualizados
            ((App)Application.Current).LoadTarefas();
            ((App)Application.Current).LoadAlunos();
            ((App)Application.Current).LoadNotasIndividuais();

            var tarefas = ((App)Application.Current).Tarefas?.ToList() ?? new();
            cbAvaliacoes.Items.Clear();
            foreach (var tarefa in tarefas)
                cbAvaliacoes.Items.Add(tarefa.Titulo);
            cbAvaliacoes.Items.Add("Nota Final");
            cbAvaliacoes.SelectedIndex = 0;
        }

        private void cbAvaliacoes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selecao = cbAvaliacoes.SelectedItem as string;
            if (selecao == "Nota Final")
            {
                GerarHistogramaNotaFinal();
            }
            else
            {
                GerarHistogramaAvaliacao(selecao);
            }
        }

        private void GerarHistogramaAvaliacao(string tituloTarefa)
        {
            var app = (App)Application.Current;
            var alunos = app.Alunos?.ToList() ?? new List<Aluno>();
            var notasIndividuais = app.NotasIndividuais?.ToList() ?? new List<NotaIndividual>();

            var notas = new List<double>();

            foreach (var aluno in alunos)
            {
                var nota = notasIndividuais
                    .FirstOrDefault(n => n.TituloTarefa == tituloTarefa && n.NumeroAluno == aluno.Numero);

                notas.Add(nota != null ? nota.Nota : 0.0);
            }

            GerarHistograma(notas, $"Histograma - {tituloTarefa}");
        }
        private void GerarHistogramaNotaFinal()
        {
            var app = (App)Application.Current;
            var alunos = app.Alunos?.ToList() ?? new List<Aluno>();
            var tarefas = app.Tarefas?.ToList() ?? new List<Tarefa>();
            var notasIndividuais = app.NotasIndividuais?.ToList() ?? new List<NotaIndividual>();

            var notasFinais = new List<double>();

            foreach (var aluno in alunos)
            {
                double somaPesos = 0;
                double somaNotas = 0;

                foreach (var tarefa in tarefas)
                {
                    var notaInd = notasIndividuais.FirstOrDefault(n =>
                        n.NumeroAluno == aluno.Numero && n.TituloTarefa == tarefa.Titulo);

                    if (notaInd != null)
                    {
                        somaNotas += notaInd.Nota * tarefa.Peso;
                        somaPesos += tarefa.Peso;
                    }
                }

                if (somaPesos > 0)
                {
                    double notaFinal = somaNotas / somaPesos;
                    notasFinais.Add(notaFinal);
                }
            }

            GerarHistograma(notasFinais, "Histograma - Nota Final");
        }

        private void GerarHistograma(List<double> notas, string titulo)
        {
            double min = 0;
            double max = 20;
            int numBins = 20;
            double binWidth = 1;

            // Cores do arco-íris
            var rainbowColors = new[]
            {
                OxyColor.Parse("#6A1B9A"),
                OxyColor.Parse("#7B1FA2"),
                OxyColor.Parse("#D32F2F"),
                OxyColor.Parse("#F57C00"),
                OxyColor.Parse("#FBC02D"),
                OxyColor.Parse("#689F38"),
                OxyColor.Parse("#388E3C"),
                OxyColor.Parse("#1976D2"),
                OxyColor.Parse("#29B6F6"),
                OxyColor.Parse("#26C6DA"),
                OxyColor.Parse("#26A69A"),
                OxyColor.Parse("#9CCC65"),
                OxyColor.Parse("#FFEE58"),
                OxyColor.Parse("#FFA726"),
                OxyColor.Parse("#FF7043"),
                OxyColor.Parse("#EC407A"),
                OxyColor.Parse("#AB47BC"),
                OxyColor.Parse("#7E57C2"),
                OxyColor.Parse("#42A5F5"),
                OxyColor.Parse("#4FC3F7")
            };

            var barSeries = new BarSeries
            {
                Title = "Notas",
                StrokeThickness = 0 // Sem borda preta
            };

            var labels = Enumerable.Range(0, numBins)
                .Select(i => $"{min + i}-{min + i + 1}")
                .ToList();

            for (int i = 0; i < numBins; i++)
            {
                double binStart = min + i * binWidth;
                double binEnd = binStart + binWidth;
                int freq = notas.Count(n => n >= binStart && (n < binEnd || (i == numBins - 1 && n == max)));
                var barItem = new BarItem { Value = freq, Color = rainbowColors[i % rainbowColors.Length] };
                barSeries.Items.Add(barItem);
            }

            var model = new PlotModel { Title = "" };

            // Eixo Y (Notas) - CategoryAxis
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                Title = "Notas",
                GapWidth = 0.1
            };
            foreach (var label in labels)
                categoryAxis.Labels.Add(label);

            // Eixo X (Frequência) - LinearAxis
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Frequência",
                Minimum = 0,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };

            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.Series.Add(barSeries);

            plotView.Model = model;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}