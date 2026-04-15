using System;
using System.Collections.Generic;
using System.Linq;
using UniversitetSystem.Models;

namespace UniversitetSystem.Services
{
    /// <summary>
    /// Hovedsystemet som koordinerer alle tjenester.
    /// Fungerer som et sentralt punkt for tilgang til kurs, bibliotek og autentisering.
    /// </summary>
    public class UniversitySystem
    {
        private readonly List<User> alleBrukere;

        public KursService KursService { get; private set; }
        public BibliotekService BibliotekService { get; private set; }
        public InnloggingService InnloggingService { get; private set; }

        public UniversitySystem()
        {
            alleBrukere = new List<User>();
            KursService = new KursService();
            BibliotekService = new BibliotekService();
            InnloggingService = new InnloggingService(alleBrukere);
        }

        /// <summary>
        /// Legger til en student direkte (brukes av SeedData).
        /// </summary>
        public void LeggTilStudent(Student student)
        {
            if (!alleBrukere.Any(b => b.GetUserId() == student.GetUserId()))
                alleBrukere.Add(student);
        }

        /// <summary>
        /// Legger til en ansatt direkte (brukes av SeedData).
        /// </summary>
        public void LeggTilAnsatt(Ansatt ansatt)
        {
            if (!alleBrukere.Any(b => b.GetUserId() == ansatt.GetUserId()))
                alleBrukere.Add(ansatt);
        }

        /// <summary>
        /// Finner en student basert på StudentID.
        /// </summary>
        public Student FinnStudent(string studentId)
        {
            return alleBrukere.OfType<Student>()
                .FirstOrDefault(s => s.StudentID.Equals(studentId,
                    StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finner en ansatt basert på AnsattID.
        /// </summary>
        public Ansatt FinnAnsatt(string ansattId)
        {
            return alleBrukere.OfType<Ansatt>()
                .FirstOrDefault(a => a.AnsattID.Equals(ansattId,
                    StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finner en bruker (student eller ansatt) basert på ID.
        /// </summary>
        public User FinnBruker(string brukerId)
        {
            return alleBrukere.FirstOrDefault(b =>
                b.GetUserId().Equals(brukerId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Henter alle brukere i systemet.
        /// </summary>
        public List<User> HentAlleBrukere()
        {
            return alleBrukere.ToList();
        }
    }
}
