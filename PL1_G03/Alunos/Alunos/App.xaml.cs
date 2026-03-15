using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
//
namespace Alunos
{
    public partial class App : Application, INotifyPropertyChanged
    {
        private Utilizador _utilizador;
        public Utilizador Utilizador
        {
            get => _utilizador;
            set
            {
                if (_utilizador != value)
                {
                    _utilizador = value;
                    OnPropertyChanged(nameof(Utilizador));
                }
            }
        }

        public ObservableCollection<Aluno> Alunos { get; set; }
        public ObservableCollection<Tarefa> Tarefas { get; set; }
        public ObservableCollection<Grupo> Grupos { get; set; }

        public ObservableCollection<NotaIndividual> NotasIndividuais { get; set; }


        public App()
        {
            LoadUtilizador();
            LoadAlunos();   // <- Primeiro
            LoadGrupos();   // <- Depois
            LoadTarefas();
            LoadNotasIndividuais();
        }

        // Utilizador
        public void SaveUtilizador()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Perfil.json");
            File.WriteAllText(filePath, JsonSerializer.Serialize(Utilizador));
        }

        public void LoadUtilizador()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Perfil.json");
            if (File.Exists(filePath))
                Utilizador = JsonSerializer.Deserialize<Utilizador>(File.ReadAllText(filePath));
            else
                Utilizador = new Utilizador();

            // Se o nome estiver vazio, assume o nome do utilizador do Windows
            if (string.IsNullOrWhiteSpace(Utilizador.Nome))
                Utilizador.Nome = Environment.UserName;
        }

        // Alunos
        public void RemoverAlunosInexistentesDosGrupos()
        {
            var numerosAlunosExistentes = Alunos.Select(a => a.Numero).ToHashSet();
            foreach (var grupo in Grupos)
            {
                if (grupo.NumerosAlunos != null)
                {
                    grupo.NumerosAlunos = grupo.NumerosAlunos
                        .Where(n => numerosAlunosExistentes.Contains(n))
                        .ToList();
                }
            }
        }
        public void SaveAlunos()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Alunos.json");
            File.WriteAllText(filePath, JsonSerializer.Serialize(Alunos), System.Text.Encoding.UTF8);
        }

        public void LoadAlunos()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Alunos.json");
            if (File.Exists(filePath))
                Alunos = JsonSerializer.Deserialize<ObservableCollection<Aluno>>(File.ReadAllText(filePath));
            else
                Alunos = new ObservableCollection<Aluno>();
        }       

        // Tarefas
        public void SaveTarefas()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Tarefas.json");
            File.WriteAllText(filePath, JsonSerializer.Serialize(Tarefas));
        }

        public void LoadTarefas()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Tarefas.json");
            if (File.Exists(filePath))
                Tarefas = JsonSerializer.Deserialize<ObservableCollection<Tarefa>>(File.ReadAllText(filePath));
            else
                Tarefas = new ObservableCollection<Tarefa>();
        }

        // Grupos
        public void SaveGrupos()
        {
            RemoverGruposVazios();
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Grupos.json");
            File.WriteAllText(filePath, JsonSerializer.Serialize(Grupos));
        }

        public void LoadGrupos()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "Grupos.json");
            if (File.Exists(filePath))
                Grupos = JsonSerializer.Deserialize<ObservableCollection<Grupo>>(File.ReadAllText(filePath));
            else
                Grupos = new ObservableCollection<Grupo>();

            // Se não houver alunos, remova todos os grupos
            if (Alunos == null || Alunos.Count == 0)
            {
                Grupos.Clear();
            }
            else
            {
                var numerosAlunosExistentes = Alunos.Select(a => a.Numero).ToHashSet();
                for (int i = Grupos.Count - 1; i >= 0; i--)
                {
                    var grupo = Grupos[i];
                    if (grupo.NumerosAlunos != null)
                    {
                        grupo.NumerosAlunos = grupo.NumerosAlunos
                            .Where(n => numerosAlunosExistentes.Contains(n))
                            .ToList();
                    }
                    // Só remove o grupo se ficou sem alunos
                    if (grupo.NumerosAlunos == null || grupo.NumerosAlunos.Count == 0)
                    {
                        Grupos.RemoveAt(i);
                    }
                }
            }
            SaveGrupos(); // Atualiza o ficheiro
        }
        public void RemoverGruposVazios()
        {
            for (int i = Grupos.Count - 1; i >= 0; i--)
            {
                var grupo = Grupos[i];
                if (grupo.NumerosAlunos == null || grupo.NumerosAlunos.Count == 0)
                    Grupos.RemoveAt(i);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));




        public void SaveNotasIndividuais()
        {
            // Classe auxiliar apenas para serialização do agrupador
            var agrupadas = NotasIndividuais
                .GroupBy(n => n.NumeroAluno)
                .Select(g => new
                {
                    NumeroAluno = g.Key,
                    Notas = g.Select(n => new NotaIndividual
                    {
                        NumeroAluno = g.Key,
                        TituloTarefa = n.TituloTarefa,
                        Nota = n.Nota,
                        StatusNota = n.StatusNota
                    }).ToList()
                }).ToList();

            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "NotasIndividuais.json");
            File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(
                agrupadas,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            ));
        }



        public void LoadNotasIndividuais()
        {
            string pastaPessoal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(pastaPessoal, "NotasIndividuais.json");
            if (File.Exists(filePath))
            {
                var agrupadas = System.Text.Json.JsonSerializer.Deserialize<List<NotaIndividual>>(File.ReadAllText(filePath));
                // Desagrupar para uso interno, com verificação de null em Notas
                NotasIndividuais = new ObservableCollection<NotaIndividual>(
                    agrupadas.SelectMany(ni =>
                        ni.Notas?.Select(n =>
                        {
                            n.NumeroAluno = ni.NumeroAluno;
                            return n;
                        }) ?? Enumerable.Empty<NotaIndividual>()
                    )
                );
            }
            else
            {
                NotasIndividuais = new ObservableCollection<NotaIndividual>();
            }
        }

    }
}


