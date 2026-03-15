using System.Collections.Generic;

namespace Alunos
{
    public class NotaIndividual
    {
        public int NumeroAluno { get; set; }
        public List<NotaIndividual> Notas { get; set; } // Para agrupamento

        // Estes campos só são usados quando Notas == null (ou seja, entrada simples)
        public string TituloTarefa { get; set; }
        public double Nota { get; set; }
        public string StatusNota { get; set; } // "F", "A" ou vazio

        public NotaIndividual() { }

        public NotaIndividual(int numeroAluno, string tituloTarefa, double nota, string statusNota = "")
        {
            NumeroAluno = numeroAluno;
            TituloTarefa = tituloTarefa;
            Nota = nota;
            StatusNota = statusNota;
        }
    }
}
