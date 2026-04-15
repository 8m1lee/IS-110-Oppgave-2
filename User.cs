using System;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Abstrakt baseklasse for alle brukere i systemet.
    /// Inneholder felles egenskaper som navn, email, brukernavn og passord.
    /// Kan ikke instansieres direkte - må arves av Student eller Ansatt.
    /// </summary>
    public abstract class User
    {
        public string Navn { get; set; }
        public string Email { get; set; }
        public string Brukernavn { get; set; }
        public string Passord { get; set; }
        public Rolle Rolle { get; set; }

        // Constructor
        protected User(string navn, string email, string brukernavn, string passord, Rolle rolle)
        {
            Navn = navn;
            Email = email;
            Brukernavn = brukernavn;
            Passord = passord;
            Rolle = rolle;
        }

        /// <summary>
        /// Sjekker om oppgitt brukernavn og passord stemmer.
        /// </summary>
        public bool ValiderInnlogging(string brukernavn, string passord)
        {
            return Brukernavn.Equals(brukernavn, StringComparison.OrdinalIgnoreCase)
                && Passord == passord;
        }

        // Abstrakt metode - hver subklasse må implementere sin egen ID
        public abstract string GetUserId();

        // Virtual metode - kan overstyres av subklasser
        public virtual string GetInfo()
        {
            return $"Navn: {Navn}, Email: {Email}, Rolle: {Rolle}";
        }
    }
}
