using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;

namespace Application.Services 
{
    public class VoteService : IVoteService // El servicio de votación
    {
        private VotingSession _session = new(); // El servicio guarda la sesión en un campo privado en memoria RAM - Mientras el servidor esté corriendo, _session vive en memoria.

    public VotingSession GetCurrentSession() => _session; // El servicio devuelve la sesión actual

    // crea una nueva sesión con la pregunta y opciones dadas, y la marca como activa. Las opciones se inicializan con un conteo de 0 votos cada una.
    public void CreateSession(string question, List<string> options)
    {
        _session = new VotingSession // Crea una nueva sesión de votación
        {
            Question = question, // Establece la pregunta a votar
            IsActive = true, // Marca la sesión como activa para aceptar votos
            Options = options.ToDictionary(o => o, _ => 0)  // convierte la lista ["Sí", "No", "Tal vez"] en {"Sí": 0, "No": 0, "Tal vez": 0}
        };
    }

    // valida que la sesión esté activa y que la opción exista
    public void CastVote(string option) 
    {
        if (!_session.IsActive) return; // Si la sesión no está activa, no se aceptan votos
        if (!_session.Options.ContainsKey(option)) return; // Si la opción no existe, no se acepta el voto

        _session.Options[option]++; // Incrementa el conteo de votos para la opción dada
    }

    // recorre todas las opciones y pone el conteo en 0, sin cambiar la pregunta ni las opciones, solo resetea los votos. Esto permite mantener la misma pregunta y opciones pero empezar de nuevo el conteo.
    public void ResetSession()
    {
        foreach (var key in _session.Options.Keys.ToList()) // Se usa ToList() para evitar modificar la colección mientras se itera
            _session.Options[key] = 0; // Resetea el conteo de votos para cada opción a 0
    }
    }
}