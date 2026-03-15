using System;
using System.ComponentModel;

namespace Alunos
{
    public class Tarefa : INotifyPropertyChanged
    {
        private string _tipoTarefa;
        private string _titulo;
        private string _descricao;
        private double _peso;
        private DateTime? _dataInicio;
        private DateTime? _dataTermino;

        public string TipoTarefa
        {
            get => _tipoTarefa;
            set { _tipoTarefa = value; OnPropertyChanged(nameof(TipoTarefa)); }
        }
        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; OnPropertyChanged(nameof(Titulo)); }
        }
        public string Descricao
        {
            get => _descricao;
            set { _descricao = value; OnPropertyChanged(nameof(Descricao)); }
        }
        public double Peso
        {
            get => _peso;
            set { _peso = value; OnPropertyChanged(nameof(Peso)); }
        }
        public DateTime? DataInicio
        {
            get => _dataInicio;
            set { _dataInicio = value; OnPropertyChanged(nameof(DataInicio)); }
        }
        public DateTime? DataTermino
        {
            get => _dataTermino;
            set { _dataTermino = value; OnPropertyChanged(nameof(DataTermino)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nome) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
