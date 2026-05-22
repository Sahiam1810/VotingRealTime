using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IVoteService
    {
        VotingSession GetCurrentSession(); // Retorna la sesión de votación activa con su pregunta, opciones y conteo de votos.
        /// Retorna null si no hay ninguna sesión creada.
        void CastVote(string option); // Registra un voto para la opción indicada dentro de la sesión activa.
        void CreateSession(string question, List<string> options); //Crea una nueva sesión de votación con la pregunta y opciones dadas.
        void ResetSession(); // Reinicia los votos de la sesión actual a cero sin eliminar la pregunta ni las opciones.
    }
}
