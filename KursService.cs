using System;
using System.Collections.Generic;
using System.Linq;
using UniversitetSystem.Models;

namespace UniversitetSystem.Services
{
    /// <summary>
    /// Tjeneste for håndtering av kurs: oppretting, påmelding/avmelding,
    /// karaktersetting og pensumregistrering.
    /// </summary>
    public class KursService
    {
        private readonly List<Kurs> alleKurs;

        public KursService()
        {
            alleKurs = new List<Kurs>();
        }

        // Gir read-only tilgang til kurslisten
        public List<Kurs> AlleKurs => alleKurs;

        /// <summary>
        /// Oppretter et nytt kurs. Forhindrer duplikate kurskoder og kursnavn.
        /// </summary>
        public Kurs OpprettKurs(string kursKode, string kursNavn,
                                 int studiepoeng, int maksPlasser, string faglærerID)
        {
            // Sjekk duplikat kurskode
            if (alleKurs.Any(k => k.KursKode.Equals(kursKode, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"Et kurs med kurskode '{kursKode}' eksisterer allerede.");
            }

            // Sjekk duplikat kursnavn
            if (alleKurs.Any(k => k.KursNavn.Equals(kursNavn, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"Et kurs med navn '{kursNavn}' eksisterer allerede.");
            }

            var kurs = new Kurs(kursKode, kursNavn, studiepoeng, maksPlasser, faglærerID);
            alleKurs.Add(kurs);
            return kurs;
        }

        /// <summary>
        /// Melder en student på et kurs. Forhindrer duplikat-påmelding.
        /// </summary>
        public bool MeldPåKurs(string kursKode, Student student)
        {
            var kurs = FinnKurs(kursKode);
            if (kurs == null)
                throw new KeyNotFoundException($"Kurs med kode '{kursKode}' finnes ikke.");

            if (student.ErPåmeldt(kursKode))
                throw new InvalidOperationException(
                    $"Studenten er allerede påmeldt kurset '{kursKode}'.");

            if (!kurs.HarLedigPlass())
                throw new InvalidOperationException(
                    $"Kurset '{kursKode}' er fullt ({kurs.MaksAntallPlasser} plasser).");

            return kurs.MeldPåStudent(student);
        }

        /// <summary>
        /// Melder en student av et kurs.
        /// </summary>
        public bool MeldAvKurs(string kursKode, Student student)
        {
            var kurs = FinnKurs(kursKode);
            if (kurs == null)
                throw new KeyNotFoundException($"Kurs med kode '{kursKode}' finnes ikke.");

            if (!student.ErPåmeldt(kursKode))
                throw new InvalidOperationException(
                    $"Studenten er ikke påmeldt kurset '{kursKode}'.");

            return kurs.MeldAvStudent(student);
        }

        /// <summary>
        /// Setter karakter for en student i et kurs. Kun faglæreren kan sette karakter.
        /// </summary>
        public bool SettKarakter(string kursKode, string studentId,
                                  string bokstavkarakter, string faglærerID)
        {
            var kurs = FinnKurs(kursKode);
            if (kurs == null)
                throw new KeyNotFoundException($"Kurs med kode '{kursKode}' finnes ikke.");

            if (!kurs.FaglærerID.Equals(faglærerID, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(
                    "Kun faglæreren for dette kurset kan sette karakterer.");

            if (!Karakter.ErGyldigKarakter(bokstavkarakter))
                throw new ArgumentException(
                    $"'{bokstavkarakter}' er ikke en gyldig karakter. Bruk A-F.");

            return kurs.SettKarakter(studentId, bokstavkarakter, faglærerID);
        }

        /// <summary>
        /// Registrerer en bok som pensum for et kurs.
        /// </summary>
        public bool RegistrerPensum(string kursKode, string bokId, string faglærerID)
        {
            var kurs = FinnKurs(kursKode);
            if (kurs == null)
                throw new KeyNotFoundException($"Kurs med kode '{kursKode}' finnes ikke.");

            if (!kurs.FaglærerID.Equals(faglærerID, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(
                    "Kun faglæreren for dette kurset kan registrere pensum.");

            return kurs.LeggTilPensum(bokId);
        }

        /// <summary>
        /// Finner et kurs basert på kurskode.
        /// </summary>
        public Kurs FinnKurs(string kursKode)
        {
            return alleKurs.FirstOrDefault(k =>
                k.KursKode.Equals(kursKode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Søker etter kurs basert på kurskode eller kursnavn.
        /// </summary>
        public List<Kurs> SøkKurs(string søkeord)
        {
            if (string.IsNullOrWhiteSpace(søkeord))
                return new List<Kurs>();

            string søk = søkeord.ToLower();
            return alleKurs.Where(k =>
                k.KursKode.ToLower().Contains(søk) ||
                k.KursNavn.ToLower().Contains(søk))
                .ToList();
        }

        /// <summary>
        /// Henter alle kurs som en bestemt faglærer underviser.
        /// </summary>
        public List<Kurs> HentFaglærerKurs(string faglærerID)
        {
            return alleKurs.Where(k =>
                k.FaglærerID.Equals(faglærerID, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Skriver ut alle kurs med deltakerliste til konsollen.
        /// </summary>
        public void VisAlleKurs()
        {
            if (alleKurs.Count == 0)
            {
                Console.WriteLine("Ingen kurs registrert i systemet.");
                return;
            }

            foreach (var kurs in alleKurs)
            {
                Console.WriteLine($"\n{kurs.GetInfo()}");
                if (kurs.PåmeldteStudenter.Count > 0)
                {
                    Console.WriteLine("  Studenter:");
                    foreach (var student in kurs.PåmeldteStudenter)
                    {
                        string karakter = kurs.Karakterer.ContainsKey(student.StudentID)
                            ? kurs.Karakterer[student.StudentID].Bokstavkarakter
                            : "Ikke satt";
                        Console.WriteLine($"    - {student.Navn} ({student.StudentID}) " +
                                          $"[Karakter: {karakter}]");
                    }
                }
                else
                {
                    Console.WriteLine("  Ingen studenter påmeldt.");
                }

                if (kurs.PensumBøker.Count > 0)
                {
                    Console.WriteLine($"  Pensum: {string.Join(", ", kurs.PensumBøker)}");
                }
            }
        }
    }
}
