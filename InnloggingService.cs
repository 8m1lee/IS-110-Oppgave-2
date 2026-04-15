using System;
using System.Collections.Generic;
using System.Linq;
using UniversitetSystem.Models;

namespace UniversitetSystem.Services
{
    /// <summary>
    /// Håndterer autentisering: innlogging og registrering av nye brukere.
    /// Genererer unike ID-er for studenter og ansatte.
    /// </summary>
    public class InnloggingService
    {
        private readonly List<User> alleBrukere;

        public InnloggingService(List<User> brukere)
        {
            alleBrukere = brukere;
        }

        /// <summary>
        /// Beregner neste ledige StudentID dynamisk fra listen.
        /// Skanner listen hver gang for å unngå kollisjon med seed-data.
        /// </summary>
        private string NesteStudentId()
        {
            int maks = 0;
            foreach (var bruker in alleBrukere)
            {
                if (bruker is Student s && s.StudentID.StartsWith("S"))
                {
                    if (int.TryParse(s.StudentID.Substring(1), out int nr) && nr > maks)
                        maks = nr;
                }
            }
            return $"S{(maks + 1):D3}";
        }

        /// <summary>
        /// Beregner neste ledige AnsattID dynamisk fra listen.
        /// </summary>
        private string NesteAnsattId()
        {
            int maks = 0;
            foreach (var bruker in alleBrukere)
            {
                if (bruker is Ansatt a && a.AnsattID.StartsWith("A"))
                {
                    if (int.TryParse(a.AnsattID.Substring(1), out int nr) && nr > maks)
                        maks = nr;
                }
            }
            return $"A{(maks + 1):D3}";
        }

        /// <summary>
        /// Forsøker innlogging med brukernavn og passord.
        /// Returnerer brukerobjektet hvis vellykket, ellers null.
        /// </summary>
        public User LoggInn(string brukernavn, string passord)
        {
            if (string.IsNullOrWhiteSpace(brukernavn) || string.IsNullOrWhiteSpace(passord))
                return null;

            return alleBrukere.FirstOrDefault(b => b.ValiderInnlogging(brukernavn, passord));
        }

        /// <summary>
        /// Sjekker om et brukernavn allerede er tatt.
        /// </summary>
        public bool BrukernavnEksisterer(string brukernavn)
        {
            return alleBrukere.Any(b =>
                b.Brukernavn.Equals(brukernavn, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Registrerer en ny student i systemet.
        /// </summary>
        public Student RegistrerStudent(string navn, string email,
                                         string brukernavn, string passord)
        {
            if (BrukernavnEksisterer(brukernavn))
                throw new InvalidOperationException("Brukernavnet er allerede i bruk.");

            string studentId = NesteStudentId();

            var student = new Student(studentId, navn, email, brukernavn, passord);
            alleBrukere.Add(student);
            return student;
        }

        /// <summary>
        /// Registrerer en ny utvekslingsstudent i systemet.
        /// </summary>
        public UtvekslingStudent RegistrerUtvekslingsStudent(
            string navn, string email, string brukernavn, string passord,
            string hjemuniversitet, string land,
            DateTime periodeFra, DateTime periodeTil)
        {
            if (BrukernavnEksisterer(brukernavn))
                throw new InvalidOperationException("Brukernavnet er allerede i bruk.");

            string studentId = NesteStudentId();

            var student = new UtvekslingStudent(
                studentId, navn, email, brukernavn, passord,
                hjemuniversitet, land, periodeFra, periodeTil);
            alleBrukere.Add(student);
            return student;
        }

        /// <summary>
        /// Registrerer en ny ansatt (faglærer eller bibliotekar) i systemet.
        /// </summary>
        public Ansatt RegistrerAnsatt(string navn, string email,
                                       string brukernavn, string passord,
                                       string stilling, string avdeling, Rolle rolle)
        {
            if (rolle == Rolle.Student)
                throw new ArgumentException("Bruk RegistrerStudent for studenter.");

            if (BrukernavnEksisterer(brukernavn))
                throw new InvalidOperationException("Brukernavnet er allerede i bruk.");

            string ansattId = NesteAnsattId();

            var ansatt = new Ansatt(ansattId, navn, email, brukernavn, passord,
                                    stilling, avdeling, rolle);
            alleBrukere.Add(ansatt);
            return ansatt;
        }
    }
}
