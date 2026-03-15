using System.Collections.Generic;
using System.Linq;

namespace Alunos
{
    public class Grupo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<int> NumerosAlunos { get; set; } = new();

        // Propriedade auxiliar para mostrar nomes dos alunos (não serializada)
        public List<string> ElementosNomes
        {
            get
            {
                var app = System.Windows.Application.Current as App;
                if (app?.Alunos == null) return new List<string>();
                return app.Alunos
                    .Where(a => NumerosAlunos.Contains(a.Numero))
                    .Select(a => a.Nome)
                    .ToList();
            }
        }
    }
}
