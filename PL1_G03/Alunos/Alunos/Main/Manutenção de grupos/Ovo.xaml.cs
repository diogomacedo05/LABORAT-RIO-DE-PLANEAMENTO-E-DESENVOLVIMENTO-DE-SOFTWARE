using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Alunos
{
    /// <summary>
    /// Lógica interna para Ovo.xaml
    /// </summary>
    public partial class Ovo : Window
    {
        public Ovo()
        {
            InitializeComponent();

            string caminhoVideo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "videoplayback.mp4");
            mediaElement.Source = new Uri(caminhoVideo, UriKind.Absolute);
            mediaElement.MediaEnded += MediaElement_MediaEnded; // Adiciona o evento para repetir
            mediaElement.Play();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.Zero; // Volta ao início
            mediaElement.Play();                   // Reproduz novamente
        }
    }
}
