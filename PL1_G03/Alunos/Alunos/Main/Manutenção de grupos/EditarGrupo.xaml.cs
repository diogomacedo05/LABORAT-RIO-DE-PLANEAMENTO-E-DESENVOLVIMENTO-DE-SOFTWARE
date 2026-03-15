using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Alunos
{
    public partial class EditarGrupo : Window
    {
        private bool guardado = false;
        private int grupoId;
        private Grupo grupoOriginal;
        private List<ElementoGrupo> membrosOriginais = new();

        public EditarGrupo(int grupoId, List<Grupo> grupos)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            this.grupoId = grupoId;

            grupoOriginal = ((App)Application.Current).Grupos.FirstOrDefault(g => g.Id == grupoId);

            if (grupoOriginal != null)
            {
                var alunos = ((App)Application.Current).Alunos;
                var elementos = grupoOriginal.NumerosAlunos
                    .Select(num => alunos.FirstOrDefault(a => a.Numero == num))
                    .Where(a => a != null)
                    .Select(a => new ElementoGrupo { Numero = a.Numero, Nome = a.Nome })
                    .ToList();

                membrosOriginais = elementos.Select(e => new ElementoGrupo { Numero = e.Numero, Nome = e.Nome }).ToList();
                AtualizarDataGridEditar(elementos);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!guardado)
                RestaurarMembrosOriginais();
        }

        private void AtualizarDataGridEditar(List<ElementoGrupo> novaLista)
        {
            var selecionados = dataGridEditar.SelectedItems.Cast<ElementoGrupo>().Select(e => e.Numero).ToList();

            dataGridEditar.ItemsSource = null;
            dataGridEditar.ItemsSource = novaLista;

            dataGridEditar.SelectedItems.Clear();
            foreach (var numero in selecionados)
            {
                var item = novaLista.FirstOrDefault(e => e.Numero == numero);
                if (item != null)
                    dataGridEditar.SelectedItems.Add(item);
            }
        }

        private void RestaurarMembrosOriginais()
        {
            if (grupoOriginal != null)
            {
                grupoOriginal.NumerosAlunos = membrosOriginais.Select(e => e.Numero).ToList();
                ((App)Application.Current).SaveGrupos();
            }
        }

        private void btnEditar1_Click(object sender, RoutedEventArgs e)
        {
            var listaAtual = (dataGridEditar.ItemsSource as List<ElementoGrupo>) ?? new List<ElementoGrupo>();
            var alunosGlobais = ((App)Application.Current).Alunos;

            // Aqui pode abrir uma janela de seleção de alunos, ou adicionar manualmente
            var janelaAdicionar = new AdicionarAlunoGrupo(listaAtual.Count);
            if (janelaAdicionar.ShowDialog() == true)
            {
                var alunosSelecionados = janelaAdicionar.AlunosSelecionados ?? new List<ElementoGrupo>();

                foreach (var aluno in alunosSelecionados)
                {
                    if (!listaAtual.Any(e => e.Numero == aluno.Numero))
                    {
                        listaAtual.Add(new ElementoGrupo
                        {
                            Numero = aluno.Numero,
                            Nome = aluno.Nome
                        });
                    }
                }

                AtualizarDataGridEditar(listaAtual);
            }
        }

        private void BtnApagar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ElementoGrupo elemento)
            {
                var lista = dataGridEditar.ItemsSource as List<ElementoGrupo>;
                if (lista != null)
                {
                    var novaLista = lista.Where(e => e != elemento).ToList();
                    AtualizarDataGridEditar(novaLista);
                }
            }
        }
        private void RemoverNotasGrupo(IEnumerable<int> numerosAlunos)
        {
            var app = (App)Application.Current;
            var numerosSet = numerosAlunos.ToHashSet();

            // Obtenha os títulos das tarefas do tipo "Grupo"
            var titulosTrabalhosGrupo = app.Tarefas
                .Where(t => t.TipoTarefa == "Trabalho de Grupo")
                .Select(t => t.Titulo)
                .ToHashSet();

            // Remove apenas as notas dos alunos para trabalhos de grupo
            app.NotasIndividuais = new System.Collections.ObjectModel.ObservableCollection<NotaIndividual>(
                app.NotasIndividuais.Where(n =>
                    !(numerosSet.Contains(n.NumeroAluno) && titulosTrabalhosGrupo.Contains(n.TituloTarefa))
                )
            );
            app.SaveNotasIndividuais();
        }
        private void btnEditar4_Click(object sender, RoutedEventArgs e)
        {
            var elementosAtualizados = new List<ElementoGrupo>();
            foreach (var item in dataGridEditar.ItemsSource)
            {
                if (item is ElementoGrupo eg)
                    elementosAtualizados.Add(eg);
            }

            if (grupoOriginal != null)
            {
                var alunosAntes = membrosOriginais.Select(e => e.Numero).ToList();
                var alunosDepois = elementosAtualizados.Select(e => e.Numero).ToList();

                // Alunos que saíram do grupo
                var alunosRemovidos = alunosAntes.Except(alunosDepois).ToList();

                grupoOriginal.NumerosAlunos = alunosDepois;

                if (alunosRemovidos.Count > 0)
                {
                    RemoverNotasGrupo(alunosRemovidos);
                }

                if (grupoOriginal.NumerosAlunos.Count == 0)
                {
                    ((App)Application.Current).Grupos.Remove(grupoOriginal);
                    MessageBox.Show("Grupo removido com sucesso!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Alterações guardadas com sucesso!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                ((App)Application.Current).RemoverAlunosInexistentesDosGrupos();
                ((App)Application.Current).SaveGrupos();
                guardado = true;
                this.Close();
            }
        }

        private void btnEditar3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGridEditar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }

}
