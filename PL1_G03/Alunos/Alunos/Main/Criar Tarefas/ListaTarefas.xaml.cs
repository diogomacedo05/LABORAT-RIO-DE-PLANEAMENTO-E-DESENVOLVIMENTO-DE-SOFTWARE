using System.Windows;
using System.Windows.Controls;

namespace Alunos
{
    public partial class ListaTarefas : Window
    {
        private System.Collections.ObjectModel.ObservableCollection<Tarefa> tarefas => ((App)Application.Current).Tarefas;

        public ListaTarefas()
        {
            InitializeComponent();
            DataContext = (App)Application.Current; // Para binding, se usar no XAML
            CarregarTarefas();
        }

        private void CarregarTarefas()
        {
            // Usa a lista global de tarefas
            lvTarefas.ItemsSource = tarefas;
        }

        private void btnCriarTarefa_Click_1(object sender, RoutedEventArgs e)
        {
            CriarTarefa criarTarefa = new CriarTarefa();
            criarTarefa.ShowDialog();

            // Atualiza a lista após criar uma nova tarefa
            CarregarTarefas();
            ((App)Application.Current).SaveTarefas();
            
        }

        private void btnEditarTarefa_Click_1(object sender, RoutedEventArgs e)
        {
            if (lvTarefas.SelectedItem is Tarefa tarefaSelecionada)
            {
                // Cria uma cópia para edição
                var copiaTarefa = new Tarefa
                {
                    Titulo = tarefaSelecionada.Titulo,
                    Descricao = tarefaSelecionada.Descricao,
                    Peso = tarefaSelecionada.Peso,
                    DataInicio = tarefaSelecionada.DataInicio,
                    DataTermino = tarefaSelecionada.DataTermino,
                    TipoTarefa = tarefaSelecionada.TipoTarefa
                };
                EditarTarefa editarTarefaWindow = new EditarTarefa(copiaTarefa);
                if (editarTarefaWindow.ShowDialog() == true)
                {
                    // Atualiza os campos da tarefa original
                    tarefaSelecionada.Titulo = copiaTarefa.Titulo;
                    tarefaSelecionada.Descricao = copiaTarefa.Descricao;
                    tarefaSelecionada.Peso = copiaTarefa.Peso;
                    tarefaSelecionada.DataInicio = copiaTarefa.DataInicio;
                    tarefaSelecionada.DataTermino = copiaTarefa.DataTermino;
                    tarefaSelecionada.TipoTarefa = copiaTarefa.TipoTarefa;
                    ((App)Application.Current).SaveTarefas();
                    CarregarTarefas();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma tarefa para editar.", "Editar Tarefa", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnEliminarTarefa_Click_1(object sender, RoutedEventArgs e)
        {
            if (lvTarefas.SelectedItem is Tarefa tarefaSelecionada)
            {
                var resultado = MessageBox.Show("Tem certeza que deseja eliminar esta tarefa?", "Confirmação", MessageBoxButton.YesNo);
                if (resultado == MessageBoxResult.Yes)
                {
                    tarefas.Remove(tarefaSelecionada);
                    ((App)Application.Current).SaveTarefas();
                    CarregarTarefas();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma tarefa para eliminar.");
            }
        }

        private void btnVoltar_Click_1(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).SaveTarefas();
            this.Close();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Ajuste para compensar a barra de rolagem vertical e margens
            double totalWidth = lvTarefas.ActualWidth - 35;
            if (totalWidth < 0) totalWidth = 0;

            // Proporções para cada coluna (ajuste conforme desejar)
            double[] proporcoes = { 0.12, 0.18, 0.28, 0.09, 0.16, 0.17 };

            var gridView = lvTarefas.View as GridView;
            if (gridView == null) return;

            for (int i = 0; i < gridView.Columns.Count && i < proporcoes.Length; i++)
            {
                gridView.Columns[i].Width = totalWidth * proporcoes[i];
            }
            }
        }

}
