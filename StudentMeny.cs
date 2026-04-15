using System;
using System.Linq;
using UniversitetSystem.Models;
using UniversitetSystem.Services;
using UniversitetSystem.Helpers;

namespace UniversitetSystem.Menyer
{
    /// <summary>
    /// Meny for studenter.
    /// Gir tilgang til: påmelding/avmelding kurs, se karakterer, søke/låne bøker,
    /// returnere bøker, og se påmeldte kurs.
    /// </summary>
    public class StudentMeny
    {
        private readonly UniversitySystem system;
        private readonly Student student;

        public StudentMeny(UniversitySystem system, Student student)
        {
            this.system = system;
            this.student = student;
        }

        /// <summary>
        /// Kjører studentmenyen i en løkke til brukeren logger ut.
        /// </summary>
        public void Kjør()
        {
            bool aktiv = true;
            while (aktiv)
            {
                Console.Clear();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine($"  STUDENTMENY - {student.Navn} ({student.StudentID})");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("[1] Meld meg på kurs");
                Console.WriteLine("[2] Meld meg av kurs");
                Console.WriteLine("[3] Se mine kurs");
                Console.WriteLine("[4] Se mine karakterer");
                Console.WriteLine("[5] Søk på kurs");
                Console.WriteLine("[6] Søk på bok");
                Console.WriteLine("[7] Lån bok");
                Console.WriteLine("[8] Returner bok");
                Console.WriteLine("[9] Se mine aktive lån");
                Console.WriteLine("[0] Logg ut");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("Velg: ");

                string valg = Console.ReadLine()?.Trim();

                try
                {
                    switch (valg)
                    {
                        case "1": MeldPåKurs(); break;
                        case "2": MeldAvKurs(); break;
                        case "3": SeMineKurs(); break;
                        case "4": SeMineKarakterer(); break;
                        case "5": SøkPåKurs(); break;
                        case "6": SøkPåBok(); break;
                        case "7": LånBok(); break;
                        case "8": ReturnerBok(); break;
                        case "9": SeMineAktiveLån(); break;
                        case "0": aktiv = false; continue;
                        default: Console.WriteLine("Ugyldig valg."); break;
                    }
                }
                catch (Exception ex)
                {
                    KonsollHjelper.VisFeil(ex.Message);
                }

                if (aktiv) KonsollHjelper.VentPåTast();
            }
        }

        private void MeldPåKurs()
        {
            KonsollHjelper.VisOverskrift("Meld på kurs");
            string kursKode = KonsollHjelper.LesTekst("Kurskode: ");
            system.KursService.MeldPåKurs(kursKode, student);
            KonsollHjelper.VisSuksess($"Du er nå meldt på kurset '{kursKode}'.");
        }

        private void MeldAvKurs()
        {
            KonsollHjelper.VisOverskrift("Meld av kurs");

            if (student.PåmeldteKurs.Count == 0)
            {
                Console.WriteLine("Du er ikke påmeldt noen kurs.");
                return;
            }

            Console.WriteLine("Dine kurs:");
            foreach (var kurs in student.PåmeldteKurs)
            {
                Console.WriteLine($"  {kurs.GetInfo()}");
            }

            string kursKode = KonsollHjelper.LesTekst("\nKurskode å melde av: ");
            system.KursService.MeldAvKurs(kursKode, student);
            KonsollHjelper.VisSuksess($"Du er nå meldt av kurset '{kursKode}'.");
        }

        private void SeMineKurs()
        {
            KonsollHjelper.VisOverskrift("Mine kurs");

            if (student.PåmeldteKurs.Count == 0)
            {
                Console.WriteLine("Du er ikke påmeldt noen kurs.");
                return;
            }

            Console.WriteLine($"Du er påmeldt {student.PåmeldteKurs.Count} kurs:\n");
            foreach (var kurs in student.PåmeldteKurs)
            {
                Console.WriteLine($"  {kurs.GetInfo()}");
                if (kurs.PensumBøker.Count > 0)
                {
                    Console.Write("    Pensum: ");
                    foreach (var bokId in kurs.PensumBøker)
                    {
                        var bok = system.BibliotekService.FinnBok(bokId);
                        Console.Write(bok != null ? $"[{bok.Tittel}] " : $"[{bokId}] ");
                    }
                    Console.WriteLine();
                }
            }
        }

        private void SeMineKarakterer()
        {
            KonsollHjelper.VisOverskrift("Mine karakterer");

            if (student.Karakterer.Count == 0)
            {
                Console.WriteLine("Du har ingen registrerte karakterer.");
                return;
            }

            Console.WriteLine(String.Format("{0,-12} {1,-25} {2,-10}",
                "Kurskode", "Kursnavn", "Karakter"));
            Console.WriteLine(new string('-', 50));

            foreach (var entry in student.Karakterer)
            {
                var kurs = system.KursService.FinnKurs(entry.Key);
                string kursNavn = kurs != null ? kurs.KursNavn : "Ukjent";
                Console.WriteLine(String.Format("{0,-12} {1,-25} {2,-10}",
                    entry.Key, kursNavn, entry.Value.Bokstavkarakter));
            }
        }

        private void SøkPåKurs()
        {
            KonsollHjelper.VisOverskrift("Søk på kurs");
            string søkeord = KonsollHjelper.LesTekst("Søkeord: ");
            var resultat = system.KursService.SøkKurs(søkeord);

            if (resultat.Count == 0)
            {
                Console.WriteLine("Ingen kurs funnet.");
                return;
            }

            Console.WriteLine($"\nFant {resultat.Count} kurs:\n");
            foreach (var kurs in resultat)
            {
                Console.WriteLine($"  {kurs.GetInfo()}");
            }
        }

        private void SøkPåBok()
        {
            KonsollHjelper.VisOverskrift("Søk på bok");
            string søkeord = KonsollHjelper.LesTekst("Søkeord (tittel, forfatter eller ID): ");
            var resultat = system.BibliotekService.SøkBok(søkeord);

            if (resultat.Count == 0)
            {
                Console.WriteLine("Ingen bøker funnet.");
                return;
            }

            Console.WriteLine($"\nFant {resultat.Count} bok(er):\n");
            foreach (var bok in resultat)
            {
                Console.WriteLine($"  {bok.GetInfo()}");
            }
        }

        private void LånBok()
        {
            KonsollHjelper.VisOverskrift("Lån bok");
            string bokId = KonsollHjelper.LesTekst("BokID: ");
            system.BibliotekService.LånBok(bokId, student);
        }

        private void ReturnerBok()
        {
            KonsollHjelper.VisOverskrift("Returner bok");

            var aktiveLån = system.BibliotekService.HentAktiveLån(student.StudentID);
            if (aktiveLån.Count == 0)
            {
                Console.WriteLine("Du har ingen aktive lån.");
                return;
            }

            Console.WriteLine("Dine aktive lån:");
            foreach (var lån in aktiveLån)
            {
                Console.WriteLine($"  [{lån.Bok.BokID}] {lån.Bok.Tittel} - Lånt: {lån.LånDato:dd.MM.yyyy}");
            }

            string bokId = KonsollHjelper.LesTekst("\nBokID å returnere: ");
            system.BibliotekService.ReturnerBok(bokId, student);
        }

        private void SeMineAktiveLån()
        {
            KonsollHjelper.VisOverskrift("Mine aktive lån");

            var aktiveLån = system.BibliotekService.HentAktiveLån(student.StudentID);
            if (aktiveLån.Count == 0)
            {
                Console.WriteLine("Du har ingen aktive lån.");
                return;
            }

            foreach (var lån in aktiveLån)
            {
                Console.WriteLine($"  {lån.GetInfo()}");
            }
        }
    }
}
