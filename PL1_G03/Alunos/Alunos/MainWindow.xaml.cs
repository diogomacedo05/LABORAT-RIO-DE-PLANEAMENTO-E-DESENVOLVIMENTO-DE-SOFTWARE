using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Alunos
{
    public partial class MainWindow : Window
    {
        private Utilizador utilizador => ((App)Application.Current).Utilizador;

        public MainWindow()
        {
            InitializeComponent();
            AtualizarLabelsPerfil();
            CarregarImagemPerfil();

            // Hook para impedir redimensionamento manual
            SourceInitialized += Window_SourceInitialized;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(WindowProc);
        }

        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 1;

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                handled = true;
                return new IntPtr(HTCLIENT);
            }

            return IntPtr.Zero;
        }

        private void CarregarImagemPerfil()
        {
            if (!string.IsNullOrEmpty(utilizador.CaminhoImagem) && System.IO.File.Exists(utilizador.CaminhoImagem))
            {
                var bitmap = new BitmapImage(new Uri(utilizador.CaminhoImagem, UriKind.Absolute));
                ImagemPerfil.Fill = new ImageBrush(bitmap);
            }
            else
            {
                ImagemPerfil.Fill = null;
            }
        }

        private void AtualizarLabelsPerfil()
        {
            NomeLabel.Content = utilizador.Nome;
            EmailLabel.Content = utilizador.Email;
        }

        private void btneditarperfil_Click(object sender, RoutedEventArgs e)
        {
            EditarPerfil janelaEditar = new EditarPerfil();
            bool? resultado = janelaEditar.ShowDialog();

            if (resultado == true)
            {
                AtualizarLabelsPerfil();
                CarregarImagemPerfil();
            }
        }

        private void btnEditarListaAlunos_Click(object sender, RoutedEventArgs e)
        {
            ListaDeAlunos1 janela = new ListaDeAlunos1();
            janela.ShowDialog();
        }

        private void btnEditarListaTarefas_Click(object sender, RoutedEventArgs e)
        {
            ListaTarefas listaTarefas = new ListaTarefas();
            listaTarefas.Show();
        }

        private void btnManutencaoGrupos_Click(object sender, RoutedEventArgs e)
        {
            ManutencaoGrupos janela = new ManutencaoGrupos();
            janela.ShowDialog();
        }

        private bool subMenuVisivel = false;

        private void btnPautaEstatistica_Click(object sender, RoutedEventArgs e)
        {
            subMenuVisivel = !subMenuVisivel;
            SubMenuPautas.Visibility = subMenuVisivel ? Visibility.Visible : Visibility.Collapsed;
            btnPautaEstatistica.Content = subMenuVisivel ? "Pauta & Estatísticas ▲" : "Pauta & Estatísticas ▼";
        }

        private void btnAbrirPauta_Click(object sender, RoutedEventArgs e)
        {
            Pauta janela = new Pauta();
            janela.ShowDialog();
        }

        private void btnEstatisticas_Click(object sender, RoutedEventArgs e)
        {
            Estatatisticas estatatisticas = new Estatatisticas();
            estatatisticas.ShowDialog();
        }

        private void btnAbrirNotas_Click(object sender, RoutedEventArgs e)
        {
            AtribuirNotas janela = new AtribuirNotas();
            janela.ShowDialog();
        }
    }
}
