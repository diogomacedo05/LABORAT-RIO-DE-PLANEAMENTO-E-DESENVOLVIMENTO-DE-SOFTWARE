using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Alunos
{
    /// <summary>
    /// Lógica interna para AdicionarAlunoGrupo.xaml
    /// </summary>
    public partial class AdicionarAlunoGrupo : Window
    {
        public List<ElementoGrupo> AlunosSelecionados { get; private set; }
        private readonly int alunosJaNoGrupo;
        private List<ElementoGrupo> todosAlunos = new List<ElementoGrupo>();

        public AdicionarAlunoGrupo(int alunosJaNoGrupo)
        {
            InitializeComponent();
            AlunosSelecionados = new List<ElementoGrupo>();
            this.alunosJaNoGrupo = alunosJaNoGrupo;

            // Usar a App.xaml.cs para obter os dados
            var app = (App)Application.Current;

            // Obter todos os números de alunos já agrupados
            HashSet<int> numerosAgrupados = app.Grupos
                .SelectMany(g => g.NumerosAlunos)
                .ToHashSet();

            // Carregar todos os alunos da App, exceto os já agrupados
            todosAlunos = app.Alunos
                .Where(a => !numerosAgrupados.Contains(a.Numero))
                .Select(a => new ElementoGrupo
                {
                    Numero = a.Numero,
                    Nome = a.Nome,
                    Selecionar = false
                })
                .ToList();

            dataGridAlunos.ItemsSource = todosAlunos;
        }

        private void btnGuardarEditar_Click(object sender, RoutedEventArgs e)
        {
            // Sempre pega a lista atualmente exibida no DataGrid
            AlunosSelecionados = todosAlunos.Where(a => a.Selecionar).ToList();

            if (AlunosSelecionados.Count + alunosJaNoGrupo > 4)
            {
                MessageBox.Show("Só pode ter até 4 alunos no grupo (incluindo os já existentes).", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return new string(texto
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToLower();
        }

        private void btnVoltarEditar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        private void tbNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarAlunos();
        }

        private void tbNumero_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarAlunos();
        }
    }
}