using System.ComponentModel;

namespace Alunos
{
    public class Utilizador : INotifyPropertyChanged
    {
        private string _nome;
        private string _email;
        private string _caminhoImagem;

        public string Nome
        {
            get => _nome;
            set
            {
                if (_nome != value)
                {
                    _nome = value;
                    OnPropertyChanged(nameof(Nome));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string CaminhoImagem
        {
            get => _caminhoImagem;
            set
            {
                if (_caminhoImagem != value)
                {
                    _caminhoImagem = value;
                    OnPropertyChanged(nameof(CaminhoImagem));
                }
            }
        }

        public Utilizador() { }

        public Utilizador(string nome, string email, string caminhoImagem)
        {
            Nome = nome;
            Email = email;
            CaminhoImagem = caminhoImagem;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
