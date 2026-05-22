using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VotingSession  // sesion de votacion
    {
        public string? Question { get; set; } // pregunta a votar
        public Dictionary<string, int> Options { get; set; } = new(); // opciones de voto y su conteo El Dictionary<string, int> es la clave: la llave (string) es el nombre de la opción ("Sí", "No") y el valor (int) es cuántos votos tiene
        public bool IsActive { get; set; } = false; // indica si la pregunta esta activa para votar
    }
}