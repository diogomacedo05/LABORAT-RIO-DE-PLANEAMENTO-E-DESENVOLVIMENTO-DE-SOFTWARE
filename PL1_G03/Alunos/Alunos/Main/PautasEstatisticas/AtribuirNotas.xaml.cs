using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Alunos
{
    public partial class AtribuirNotas : Window
    {
        private List<Aluno> alunos = new();
        private List<Tarefa> tarefas = new();
        private List<Grupo> grupos = new();

        private void RecarregarDadosGlobais()
        {
            ((App)Application.Current).LoadTarefas();
            ((App)Application.Current).LoadAlunos();
            ((App)Application.Current).LoadGrupos();
            tarefas = ((App)Application.Current).Tarefas?.ToList() ?? new();
            alunos = ((App)Application.Current).Alunos?.ToList() ?? new();
            grupos = ((App)Application.Current).Grupos?.ToList() ?? new();
            cbTarefas.ItemsSource = tarefas;
            cbGrupos.ItemsSource = grupos;
        }
        public AtribuirNotas()
        {
            InitializeComponent();
            RecarregarDadosGlobais();
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAtribuirNotaGrupo_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtNotaGrupo.Text, out double nota) || nota < 0 || nota > 20)
            {
                MessageBox.Show("Introduza uma nota válida entre 0 e 20.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var aluno in alunos)
            {
                aluno.Nota = nota;
            }
            dgAlunos.Items.Refresh();
        }

        private void btnAtribuirNota_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            if (cbTarefas.SelectedItem is not Tarefa tarefaSelecionada)
            {
                MessageBox.Show("Selecione uma tarefa.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var aluno in alunos)
            {
                if (aluno.Nota < 0 || aluno.Nota > 20)
                    continue;

                var notaInd = app.NotasIndividuais.FirstOrDefault(n =>
                    n.NumeroAluno == aluno.Numero && n.TituloTarefa == tarefaSelecionada.Titulo);

                if (notaInd == null)
                {
                    app.NotasIndividuais.Add(new NotaIndividual(aluno.Numero, tarefaSelecionada.Titulo, aluno.Nota, ""));
                }
                else
                {
                    notaInd.Nota = aluno.Nota;
                }
            }
            app.SaveNotasIndividuais();
            MessageBox.Show("Notas guardadas!");
            CarregarNotasDaTarefa(tarefaSelecionada.Titulo);
        }

        private void cbTarefas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTarefas.SelectedItem is Tarefa tarefaSelecionada)
            {
                dgAlunos.Visibility = Visibility.Visible;

                if (tarefaSelecionada.TipoTarefa?.ToLower().Contains("grupo") == true)
                {
                    cbGrupos.Visibility = Visibility.Visible;
                    dgAlunos.ItemsSource = null;
                    txtGrupoId.Text = "";
                }
                else
                {
                    cbGrupos.Visibility = Visibility.Collapsed;
                    ((App)Application.Current).LoadAlunos();
                    alunos = ((App)Application.Current).Alunos?.ToList() ?? new();
                    dgAlunos.ItemsSource = alunos;
                    CarregarNotasDaTarefa(tarefaSelecionada.Titulo);
                    txtGrupoId.Text = "";
                }
            }
            else
            {
                dgAlunos.Visibility = Visibility.Collapsed;
            }
        }

        private void cbGrupos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbGrupos.SelectedItem is Grupo grupoSelecionado && cbTarefas.SelectedItem is Tarefa tarefaSelecionada)
            {
                var app = (App)Application.Current;
                app.LoadAlunos();
                alunos = app.Alunos.Where(a => grupoSelecionado.NumerosAlunos.Contains(a.Numero)).ToList();
                dgAlunos.ItemsSource = alunos;
                CarregarNotasDaTarefa(tarefaSelecionada.Titulo);
                txtGrupoId.Text = grupoSelecionado.Id.ToString();
            }
            else
            {
                txtGrupoId.Text = "";
            }
        }

        private void CarregarNotasDaTarefa(string tituloTarefa)
        {
            var app = (App)Application.Current;
            var notas = app.NotasIndividuais;

            foreach (Aluno aluno in alunos)
            {
                var notaInd = notas.FirstOrDefault(n => n.NumeroAluno == aluno.Numero && n.TituloTarefa == tituloTarefa);
                if (notaInd != null)
                {
                    aluno.Nota = notaInd.Nota;
                }
                else
                {
                    aluno.Nota = 0.0;
                }
            }
            dgAlunos.Items.Refresh();
        }

        private void dgAlunos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgAlunos.SelectedItem is Aluno alunoSelecionado)
            {
                dgAlunos.CommitEdit(DataGridEditingUnit.Row, true);
                dgAlunos.CommitEdit();

                var input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Atribuir nota para {alunoSelecionado.Nome} ({alunoSelecionado.Numero}):\n(Digite um valor entre 0 e 20)",
                "Atribuir Nota",
                "");

                if (double.TryParse(input, out double novaNota) && novaNota >= 0 && novaNota <= 20)
                {
                    alunoSelecionado.Nota = novaNota;
                    dgAlunos.Items.Refresh();
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Nota inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}