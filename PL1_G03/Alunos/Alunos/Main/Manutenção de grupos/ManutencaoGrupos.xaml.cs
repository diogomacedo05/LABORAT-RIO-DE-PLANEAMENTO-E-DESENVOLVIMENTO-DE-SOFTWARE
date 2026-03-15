using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Alunos
{
    public partial class ManutencaoGrupos : Window
    {
        private ObservableCollection<Grupo> grupos => ((App)Application.Current).Grupos;
        private ObservableCollection<Aluno> alunos => ((App)Application.Current).Alunos;

        public ManutencaoGrupos()
        {
            InitializeComponent();
            DataContext = Application.Current as App;
            RecarregarDados();
            
        }

        private void RecarregarDados()
        {
            ((App)Application.Current).RemoverGruposVazios();
            ((App)Application.Current).SaveGrupos(); // Salva imediatamente após remover
            dataGridGrupos.ItemsSource = null;
            dataGridGrupos.ItemsSource = grupos;
        }

        private void btnGrupo1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEditarGrupo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Grupo grupoSelecionado)
            {
                EditarGrupo janela = new EditarGrupo(grupoSelecionado.Id, grupos.ToList());
                janela.ShowDialog();
                ((App)Application.Current).SaveGrupos();
                RecarregarDados();
            }
        }

        private void btnGrupo3_Click(object sender, RoutedEventArgs e)
        {
            CriarGrupo janela = new CriarGrupo();
            janela.ShowDialog();
            ((App)Application.Current).SaveGrupos();
            RecarregarDados();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Ovo o = new Ovo();
            o.ShowDialog();
        }
    }
}
