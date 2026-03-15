using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Text;

namespace Alunos
{
    public partial class CriarGrupo : Window
    {
        private List<Aluno> todosAlunos = new List<Aluno>();
        private List<Aluno> alunosSelecionados = new List<Aluno>();

        public CriarGrupo()
        {
            InitializeComponent();
            todosAlunos = CarregarAlunosDisponiveis();
            dataGridAlunos.ItemsSource = todosAlunos;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            alunosSelecionados = todosAlunos.Where(a => a.IsSelected).ToList();

            if (alunosSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos 1 aluno para criar o grupo.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (alunosSelecionados.Count > 4)
            {
                MessageBox.Show("Só pode selecionar no máximo 4 alunos para criar o grupo.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var app = (App)Application.Current;

            // Gera novo ID
            int novoId = 1;
            if (app.Grupos.Any())
            {
                novoId = app.Grupos.Max(g => g.Id) + 1;
            }

            string nomeGrupo = tbNomeGrupo.Text.Trim();
            // Se não foi atribuído nome, usa o nome default
            if (string.IsNullOrWhiteSpace(nomeGrupo))
            {
                nomeGrupo = $"Grupo {novoId}";
            }

            // Verifica nomes duplicados
            if (app.Grupos.Any(g => g.Nome.Equals(nomeGrupo, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Já existe um grupo com esse nome. Escolha outro nome.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var novoGrupo = new Grupo
            {
                Id = novoId,
                Nome = nomeGrupo,
                NumerosAlunos = alunosSelecionados.Select(a => a.Numero).ToList()
            };

            app.Grupos.Add(novoGrupo);
            app.SaveGrupos();

            // Limpa seleção dos alunos
            foreach (var aluno in todosAlunos)
            {
                aluno.IsSelected = false;
            }

            MessageBox.Show("Grupo guardado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            alunosSelecionados = todosAlunos.Where(a => a.IsSelected).ToList();
        }
        private void FiltrarAlunos()
        {
            string Nome = RemoverAcentos(tbNome.Text);
            string Numero = tbNumero.Text;

            var resultados = todosAlunos
                .Where(aluno =>
                    aluno != null &&
                    (string.IsNullOrWhiteSpace(Nome) || RemoverAcentos(aluno.Nome).Contains(Nome)) &&
                    (string.IsNullOrWhiteSpace(Numero) || aluno.Numero.ToString().Contains(Numero))
                )
                .ToList();

            dataGridAlunos.ItemsSource = null;
            dataGridAlunos.ItemsSource = resultados;
        }
        // Só alunos que ainda não estão em grupos
        private List<Aluno> CarregarAlunosDisponiveis()
        {
            var app = (App)Application.Current;
            var alunos = app.Alunos.ToList();
            var numerosAgrupados = app.Grupos.SelectMany(g => g.NumerosAlunos).ToHashSet();
            return alunos.Where(a => !numerosAgrupados.Contains(a.Numero)).ToList();
        }

        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return new string(texto
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToLower();
        }

        private void tbNomeGrupo_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarAlunos();
        }

        private void tbNumero_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarAlunos();
        }
    }
}
