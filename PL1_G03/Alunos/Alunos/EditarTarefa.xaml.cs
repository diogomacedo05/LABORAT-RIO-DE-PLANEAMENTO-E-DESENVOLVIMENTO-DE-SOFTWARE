using System;
using System.Windows;

namespace Alunos
{
    public partial class EditarTarefa : Window
    {
        private Tarefa tarefaOriginal;

        public EditarTarefa(Tarefa tarefa)
        {
            InitializeComponent();
            tarefaOriginal = tarefa;

            tbTitulo.Text = tarefa.Titulo;
            tbDescricao.Text = tarefa.Descricao;
            tbPeso.Text = tarefa.Peso.ToString();
            dateTimePickerInicio.Value = tarefa.DataInicio;
            dateTimePickerTermino.Value = tarefa.DataTermino;
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (dateTimePickerInicio.Value.HasValue && dateTimePickerTermino.Value.HasValue)
            {
                var dataInicio = dateTimePickerInicio.Value.Value;
                var dataTermino = dateTimePickerTermino.Value.Value;

                if (string.IsNullOrWhiteSpace(tbTitulo.Text))
                {
                    MessageBox.Show("A titulo é obrigátorio.", "Campo obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validação: data de término não pode ser inferior à de início
                if (dataTermino < dataInicio)
                {
                    MessageBox.Show("A data de término não pode ser inferior à data de início.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double peso = double.TryParse(tbPeso.Text, out var p) ? p : 0;

                // Soma dos pesos das tarefas já existentes
                double somaPesos = ((App)Application.Current).Tarefas.Sum(t => t.Peso);
                if (peso <= 0)
                {
                    MessageBox.Show("O peso total das tarefas tem de ser maior que 0.", "Criar Tarefa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Verifica se o novo peso ultrapassa 100
                if (somaPesos + peso > 100)
                {
                    MessageBox.Show("O peso total das tarefas não pode exceder 100%.", "Criar Tarefa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                tarefaOriginal.Titulo = tbTitulo.Text;
                tarefaOriginal.Descricao = tbDescricao.Text;
                tarefaOriginal.Peso = double.TryParse(tbPeso.Text, out var pe) ? pe : 0;
                tarefaOriginal.DataInicio = dateTimePickerInicio.Value;
                tarefaOriginal.DataTermino = dateTimePickerTermino.Value;

                // Salva as alterações no JSON global
                ((App)Application.Current).SaveTarefas();

                MessageBox.Show("Tarefa editada com sucesso!");
                this.DialogResult = true; // <- Adicione esta linha
                this.Close();
            }
        }


        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
