using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Alunos
{
    public partial class EditarPerfil : Window
    {
        private Utilizador utilizador;

        public EditarPerfil()
        {
            InitializeComponent();
            utilizador = ((App)Application.Current).Utilizador;
            TbNome.Text = utilizador.Nome;
            tbEmail.Text = utilizador.Email;
            CarregarImagemPerfil();
        }

        private void CarregarImagemPerfil()
        {
            if (!string.IsNullOrEmpty(utilizador.CaminhoImagem) && File.Exists(utilizador.CaminhoImagem))
            {
                var bitmap = new BitmapImage(new Uri(utilizador.CaminhoImagem, UriKind.Absolute));
                CanvasCarregar.Background = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
            }
            else
            {
                CanvasCarregar.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void btnCarregar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imagens (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };
            if (dlg.ShowDialog() == true)
            {
                utilizador.CaminhoImagem = dlg.FileName;
                CarregarImagemPerfil();
            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            utilizador.Nome = TbNome.Text;
            utilizador.Email = tbEmail.Text;
            ((App)Application.Current).SaveUtilizador();
            DialogResult = true;
            Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
