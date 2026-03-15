using System.Windows;

namespace Alunos
{
    public partial class EditarAluno : Window
    {
        public Aluno Aluno { get; set; }
        private bool isNovoAluno;
        private int numeroOriginal;
        public EditarAluno(Aluno aluno, bool novoAluno = false)
        {
            InitializeComponent();
            Aluno = aluno;
            isNovoAluno = novoAluno;

            TbNumero.Text = Aluno.Numero.ToString();
            TbNome.Text = Aluno.Nome;
            TbEmail.Text = Aluno.Email;

            numeroOriginal = Aluno.Numero; // <-- Adicione esta linha
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbNome.Text) ||
                string.IsNullOrWhiteSpace(TbEmail.Text) ||
                string.IsNullOrWhiteSpace(TbNumero.Text))
            {
                MessageBox.Show("Preencha todos os campos.", "Campos obrigatórios", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TbNumero.Text, out int numero))
            {
                MessageBox.Show("Número inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verifica duplicidade de número, ignorando o próprio aluno em edição
            var alunos = ((App)Application.Current).Alunos;
            bool numeroDuplicado = alunos.Any(a => a.Numero == numero && (isNovoAluno || numero != numeroOriginal));
            if (numeroDuplicado)
            {
                MessageBox.Show("Já existe um aluno com esse número.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Aluno.Numero = numero;
            Aluno.Nome = TbNome.Text;
            Aluno.Email = TbEmail.Text;
            this.DialogResult = true;
            MessageBox.Show("Operação realizada com sucesso", "Editar Aluno", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}