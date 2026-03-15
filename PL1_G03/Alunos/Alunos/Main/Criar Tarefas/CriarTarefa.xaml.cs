using System;
using System.Windows;
using System.Windows.Controls;

namespace Alunos
{
    public partial class CriarTarefa : Window
    {
        public CriarTarefa()
        {
            InitializeComponent();
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCriarTarefa1_Click(object sender, RoutedEventArgs e)
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

                var novaTarefa = new Tarefa
                {
                    Titulo = tbTitulo.Text,
                    Descricao = tbDescricao.Text,
                    Peso = peso,
                    DataInicio = dataInicio,
                    DataTermino = dataTermino,
                    TipoTarefa = (TipoTarefaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? ""
                };

                ((App)Application.Current).Tarefas.Add(novaTarefa);
                ((App)Application.Current).SaveTarefas();

                MessageBox.Show("Operação realizada com sucesso", "Criar Tarefa", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma data para início e término.", "Criar Tarefa", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

