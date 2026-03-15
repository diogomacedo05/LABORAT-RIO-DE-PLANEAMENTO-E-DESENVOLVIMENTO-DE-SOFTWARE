using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace Alunos
{
    public partial class ListaDeAlunos1 : Window
    {
        private ObservableCollection<Aluno> alunos => ((App)Application.Current).Alunos;

        public ListaDeAlunos1()
        {
            InitializeComponent();
            DataContext = (App)Application.Current; // DataContext global para binding
        }
        private void RemoverNotasDosAlunos(IEnumerable<int> numerosAlunos)
        {
            var app = (App)Application.Current;
            var numerosSet = numerosAlunos.ToHashSet();

            app.NotasIndividuais = new System.Collections.ObjectModel.ObservableCollection<NotaIndividual>(
                app.NotasIndividuais.Where(n => !numerosSet.Contains(n.NumeroAluno))
            );
            app.SaveNotasIndividuais();
        }
        private void btnAdicionarAluno_Click(object sender, RoutedEventArgs e)
        {
            var novoAluno = new Aluno();
            var janela = new EditarAluno(novoAluno, true);
            if (janela.ShowDialog() == true)
            {
                alunos.Add(novoAluno);
                ((App)Application.Current).SaveAlunos();
                
            }
        }

        private void btnEditarAluno_Click(object sender, RoutedEventArgs e)
        {
            // Permite editar apenas se UM aluno estiver selecionado
            var alunosSelecionados = alunos.Where(a => a.IsSelected).ToList();
            if (alunosSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione um aluno para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (alunosSelecionados.Count > 1)
            {
                MessageBox.Show("Selecione apenas um aluno para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var alunoSelecionado = alunosSelecionados.First();

            // Criar uma cópia para edição
            var copiaAluno = new Aluno
            {
                Nome = alunoSelecionado.Nome,
                Numero = alunoSelecionado.Numero,
                Email = alunoSelecionado.Email
            };
            var janela = new EditarAluno(copiaAluno, false);
            if (janela.ShowDialog() == true)
            {
                alunoSelecionado.Nome = copiaAluno.Nome;
                alunoSelecionado.Numero = copiaAluno.Numero;
                alunoSelecionado.Email = copiaAluno.Email;
                ((App)Application.Current).SaveAlunos();
            }
            
        }

        private void btnRemoverAluno_Click(object sender, RoutedEventArgs e)
        {
            var alunosSelecionados = alunos.Where(a => a.IsSelected).ToList();
            if (alunosSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos um aluno para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var resultado = MessageBox.Show(
                $"Tem a certeza que deseja eliminar {alunosSelecionados.Count} aluno(s)?",
                "Confirmar Eliminação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                foreach (var aluno in alunosSelecionados)
                    alunos.Remove(aluno);
                RemoverNotasDosAlunos(alunosSelecionados.Select(a => a.Numero));
                ((App)Application.Current).SaveAlunos();
                ((App)Application.Current).RemoverAlunosInexistentesDosGrupos();
                ((App)Application.Current).SaveGrupos();
            }
        }


        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            foreach (var aluno in alunos)
                aluno.IsSelected = false;
            ((App)Application.Current).SaveAlunos();
            this.Close();
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as System.Windows.Controls.CheckBox)?.IsChecked == true;
            foreach (var aluno in alunos)
                aluno.IsSelected = isChecked;
        }

        // Importa alunos de um ficheiro CSV (Nome,Numero,Email) e guarda no JSON global
        private void btnCarregarLista_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Ficheiros CSV (*.csv)|*.csv|Ficheiros Excel (*.xlsx)|*.xlsx|Todos os ficheiros (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var ext = System.IO.Path.GetExtension(openFileDialog.FileName).ToLower();
                if (ext == ".csv")
                {
                    MessageBox.Show("Operação realizada com sucesso", "Carregar Lista", MessageBoxButton.OK, MessageBoxImage.Information);
                    ImportarAlunosDeCSV(openFileDialog.FileName);
                }
                else if (ext == ".xlsx")
                {
                    MessageBox.Show("Operação realizada com sucesso", "Carregar Lista", MessageBoxButton.OK, MessageBoxImage.Information);
                    ImportarAlunosDeXLSX(openFileDialog.FileName);
                
                }
            else
                MessageBox.Show("Formato de ficheiro não suportado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportarAlunosDeCSV(string caminhoCSV)
        {
            var novaLista = new ObservableCollection<Aluno>();
            string[] linhas = null;
            var encodings = new List<Encoding>();

            // Encodings mais comuns e regionais
            encodings.Add(new UTF8Encoding(true)); // UTF-8 com BOM
            encodings.Add(new UTF8Encoding(false)); // UTF-8 sem BOM
            try { encodings.Add(Encoding.GetEncoding("Windows-1252")); } catch { }
            try { encodings.Add(Encoding.GetEncoding("ISO-8859-1")); } catch { }
            encodings.Add(Encoding.Unicode); // UTF-16 LE
            encodings.Add(Encoding.BigEndianUnicode); // UTF-16 BE
            encodings.Add(Encoding.ASCII);
            encodings.Add(Encoding.UTF7);
            encodings.Add(Encoding.UTF32);
            // imperialis codings
            foreach (var enc in encodings)
            {
                try
                {
                    linhas = File.ReadAllLines(caminhoCSV, enc);
                    // Se não houver caracteres de substituição, aceita
                    if (!linhas.Any(l => l.Contains('�')))
                        break;
                }
                catch
                {
                    linhas = null;
                }
            }

            if (linhas == null)
            {
                MessageBox.Show("Não foi possível ler o ficheiro CSV com um encoding suportado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var linha in linhas)
            {
                var partes = linha.Split(';', ',', '\t');
                if (partes.Length >= 3 && int.TryParse(partes[1], out int numero))
                {
                    novaLista.Add(new Aluno
                    {
                        Nome = partes[0],
                        Numero = numero,
                        Email = partes[2]
                    });
                }
            }

            alunos.Clear();
            foreach (var aluno in novaLista)
                alunos.Add(aluno);

            ((App)Application.Current).SaveAlunos();
        }
        private void ImportarAlunosDeXLSX(string caminhoXLSX)
        {
            var novaLista = new ObservableCollection<Aluno>();

            using (var workbook = new ClosedXML.Excel.XLWorkbook(caminhoXLSX))
            {
                var ws = workbook.Worksheets.First();
                foreach (var row in ws.RowsUsed())
                {
                    // Supondo que a primeira linha é cabeçalho, pule se necessário
                    if (row.RowNumber() == 1 && !int.TryParse(row.Cell(2).GetValue<string>(), out _))
                        continue;

                    string nome = row.Cell(1).GetValue<string>()?.Trim();
                    string numeroStr = row.Cell(2).GetValue<string>()?.Trim();
                    string email = row.Cell(3).GetValue<string>()?.Trim();

                    if (int.TryParse(numeroStr, out int numero))
                    {
                        novaLista.Add(new Aluno
                        {
                            Nome = nome,
                            Numero = numero,
                            Email = email
                        });
                    }
                }
            }

            alunos.Clear();
            foreach (var aluno in novaLista)
                alunos.Add(aluno);

            ((App)Application.Current).SaveAlunos();
        }
    }
}
