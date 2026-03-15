using System.ComponentModel;

public class Aluno : INotifyPropertyChanged
{
    private int _numero;
    private string _nome;
    private string _email;
    private bool _isSelected;
    private double _nota;

    public int Numero
    {
        get => _numero;
        set
        {
            if (_numero != value)
            {
                _numero = value;
                OnPropertyChanged(nameof(Numero));
            }
        }
    }

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

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }

    public double Nota
    {
        get => _nota;
        set
        {
            if (_nota != value)
            {
                _nota = value;
                OnPropertyChanged(nameof(Nota));
            }
        }
    }

    

  

   
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string nomePropriedade)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
    }
}