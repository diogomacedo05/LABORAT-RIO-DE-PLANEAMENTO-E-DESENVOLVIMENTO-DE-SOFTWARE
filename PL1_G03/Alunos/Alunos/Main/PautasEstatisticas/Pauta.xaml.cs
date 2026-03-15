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
    public partial class Pauta : Window
    {
        private Pautas pautasViewModel;

        public Pauta()
        {
            InitializeComponent();

            var app = (App)Application.Current;
            // Update the constructor call to match the expected parameters
            pautasViewModel = new Pautas(app.Alunos, app.Tarefas, app);
            DataContext = pautasViewModel;

            // Adiciona dinamicamente as colunas das tarefas
            int insertIndex = 2; // Depois de Nº e Nome
            foreach (var tarefa in pautasViewModel.Tarefas)
            {
                var col = new DataGridTextColumn
                {
                    Header = tarefa,
                    Binding = new Binding($"NotasPorTarefa[{tarefa}]") { StringFormat = "F2" },
                    Width = 100
                };
                dgPauta.Columns.Insert(insertIndex++, col);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
