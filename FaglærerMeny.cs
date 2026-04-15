using System;
using System.Linq;
using UniversitetSystem.Models;
using UniversitetSystem.Services;
using UniversitetSystem.Helpers;

namespace UniversitetSystem.Menyer
{
    /// <summary>
    /// Meny for faglærere.
    /// Gir tilgang til: opprette kurs, søke kurs/bøker, låne/returnere bøker,
    /// sette karakterer, registrere pensum, og se egne kurs.
    /// </summary>
    public class FaglærerMeny
    {
        private readonly UniversitySystem system;
        private readonly Ansatt faglærer;

        public FaglærerMeny(UniversitySystem system, Ansatt faglærer)
        {
            this.system = system;
            this.faglærer = faglærer;
        }

        public void Kjør()
        {
            bool aktiv = true;
            while (aktiv)
            {
                Console.Clear();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine($"  FAGLÆRERMENY - {faglærer.Navn} ({faglærer.AnsattID})");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("[1] Opprett kurs");
                Console.WriteLine("[2] Se mine kurs");
                Console.WriteLine("[3] Søk på kurs");
                Console.WriteLine("[4] Sett karakter");
                Console.WriteLine("[5] Registrer pensum til kurs");
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
                        case "1": OpprettKurs(); break;
                        case "2": SeMineKurs(); break;
                        case "3": SøkPåKurs(); break;
                        case "4": SettKarakter(); break;
                        case "5": RegistrerPensum(); break;
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

        private void OpprettKurs()
        {
            KonsollHjelper.VisOverskrift("Opprett nytt kurs");

            string kursKode = KonsollHjelper.LesTekst("Kurskode (f.eks. DAT100): ");
            string kursNavn = KonsollHjelper.LesTekst("Kursnavn: ");
            int studiepoeng = KonsollHjelper.LesTall("Studiepoeng: ");
            int maksPlasser = KonsollHjelper.LesTall("Maks antall plasser: ");

            var kurs = system.KursService.OpprettKurs(
                kursKode, kursNavn, studiepoeng, maksPlasser, faglærer.AnsattID);
            KonsollHjelper.VisSuksess($"Kurs opprettet: {kurs.GetInfo()}");
        }

        private void SeMineKurs()
        {
            KonsollHjelper.VisOverskrift("Mine kurs");

            var mineKurs = system.KursService.HentFaglærerKurs(faglærer.AnsattID);

            if (mineKurs.Count == 0)
            {
                Console.WriteLine("Du har ingen kurs.");
                return;
            }

            foreach (var kurs in mineKurs)
            {
                Console.WriteLine($"\n{kurs.GetInfo()}");

                if (kurs.PåmeldteStudenter.Count > 0)
                {
                    Console.WriteLine("  Studenter:");
                    foreach (var s in kurs.PåmeldteStudenter)
                    {
                        string karakter = kurs.Karakterer.ContainsKey(s.StudentID)
                            ? kurs.Karakterer[s.StudentID].Bokstavkarakter
                            : "Ikke satt";
                        Console.WriteLine($"    - {s.Navn} ({s.StudentID}) [Karakter: {karakter}]");
                    }
                }
                else
                {
                    Console.WriteLine("  Ingen studenter påmeldt.");
                }

                if (kurs.PensumBøker.Count > 0)
                {
                    Console.Write("  Pensum: ");
                    foreach (var bokId in kurs.PensumBøker)
                    {
                        var bok = system.BibliotekService.FinnBok(bokId);
                        Console.Write(bok != null ? $"[{bok.Tittel}] " : $"[{bokId}] ");
                    }
                    Console.WriteLine();
                }
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

        private void SettKarakter()
        {
            KonsollHjelper.VisOverskrift("Sett karakter");

            var mineKurs = system.KursService.HentFaglærerKurs(faglærer.AnsattID);
            if (mineKurs.Count == 0)
            {
                Console.WriteLine("Du har ingen kurs å sette karakter i.");
                return;
            }

            Console.WriteLine("Dine kurs:");
            foreach (var k in mineKurs)
            {
                Console.WriteLine($"  {k.GetInfo()}");
            }

            string kursKode = KonsollHjelper.LesTekst("\nKurskode: ");

            var kurs = mineKurs.FirstOrDefault(k =>
                k.KursKode.Equals(kursKode, StringComparison.OrdinalIgnoreCase));
            if (kurs == null)
            {
                KonsollHjelper.VisFeil("Du underviser ikke dette kurset.");
                return;
            }

            if (kurs.PåmeldteStudenter.Count == 0)
            {
                Console.WriteLine("Ingen studenter påmeldt dette kurset.");
                return;
            }

            Console.WriteLine($"\nStudenter i {kursKode}:");
            foreach (var s in kurs.PåmeldteStudenter)
            {
                string eksisterende = kurs.Karakterer.ContainsKey(s.StudentID)
                    ? kurs.Karakterer[s.StudentID].Bokstavkarakter
                    : "Ikke satt";
                Console.WriteLine($"  {s.StudentID} - {s.Navn} [Nåværende: {eksisterende}]");
            }

            string studentId = KonsollHjelper.LesTekst("\nStudentID: ");
            string karakter = KonsollHjelper.LesTekst("Karakter (A-F): ");

            system.KursService.SettKarakter(kursKode, studentId, karakter, faglærer.AnsattID);
            KonsollHjelper.VisSuksess($"Karakter '{karakter.ToUpper()}' satt for student {studentId} i {kursKode}.");
        }

        private void RegistrerPensum()
        {
            KonsollHjelper.VisOverskrift("Registrer pensum");

            var mineKurs = system.KursService.HentFaglærerKurs(faglærer.AnsattID);
            if (mineKurs.Count == 0)
            {
                Console.WriteLine("Du har ingen kurs.");
                return;
            }

            Console.WriteLine("Dine kurs:");
            foreach (var k in mineKurs)
            {
                Console.Write($"  {k.GetInfo()}");
                if (k.PensumBøker.Count > 0)
                    Console.Write($" | Pensum: {string.Join(", ", k.PensumBøker)}");
                Console.WriteLine();
            }

            string kursKode = KonsollHjelper.LesTekst("\nKurskode: ");
            string bokId = KonsollHjelper.LesTekst("BokID å legge til som pensum: ");

            // Verifiser at boken finnes i biblioteket
            var bok = system.BibliotekService.FinnBok(bokId);
            if (bok == null)
            {
                KonsollHjelper.VisFeil($"Bok med ID '{bokId}' finnes ikke i biblioteket.");
                return;
            }

            bool lagt = system.KursService.RegistrerPensum(kursKode, bokId, faglærer.AnsattID);
            if (lagt)
                KonsollHjelper.VisSuksess($"'{bok.Tittel}' lagt til som pensum for {kursKode}.");
            else
                KonsollHjelper.VisInfo("Boken er allerede registrert som pensum for dette kurset.");
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
            system.BibliotekService.LånBok(bokId, faglærer);
        }

        private void ReturnerBok()
        {
            KonsollHjelper.VisOverskrift("Returner bok");

            var aktiveLån = system.BibliotekService.HentAktiveLån(faglærer.AnsattID);
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
            system.BibliotekService.ReturnerBok(bokId, faglærer);
        }

        private void SeMineAktiveLån()
        {
            KonsollHjelper.VisOverskrift("Mine aktive lån");

            var aktiveLån = system.BibliotekService.HentAktiveLån(faglærer.AnsattID);
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
