using System;
using System.Linq;
using UniversitetSystem.Models;
using UniversitetSystem.Services;
using UniversitetSystem.Helpers;

namespace UniversitetSystem.Menyer
{
    /// <summary>
    /// Meny for bibliotekansatte.
    /// Gir tilgang til: registrere bøker, se aktive lån, se lånehistorikk,
    /// og søke etter bøker.
    /// </summary>
    public class BibliotekarMeny
    {
        private readonly UniversitySystem system;
        private readonly Ansatt bibliotekar;

        public BibliotekarMeny(UniversitySystem system, Ansatt bibliotekar)
        {
            this.system = system;
            this.bibliotekar = bibliotekar;
        }

        public void Kjør()
        {
            bool aktiv = true;
            while (aktiv)
            {
                Console.Clear();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine($"  BIBLIOTEKARMENY - {bibliotekar.Navn} ({bibliotekar.AnsattID})");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("[1] Registrer ny bok");
                Console.WriteLine("[2] Søk på bok");
                Console.WriteLine("[3] Se alle bøker");
                Console.WriteLine("[4] Se aktive lån");
                Console.WriteLine("[5] Se lånehistorikk");
                Console.WriteLine("[0] Logg ut");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("Velg: ");

                string valg = Console.ReadLine()?.Trim();

                try
                {
                    switch (valg)
                    {
                        case "1": RegistrerBok(); break;
                        case "2": SøkPåBok(); break;
                        case "3": SeAlleBøker(); break;
                        case "4": SeAktiveLån(); break;
                        case "5": SeLånehistorikk(); break;
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

        private void RegistrerBok()
        {
            KonsollHjelper.VisOverskrift("Registrer ny bok");

            string bokId = KonsollHjelper.LesTekst("BokID: ");
            string tittel = KonsollHjelper.LesTekst("Tittel: ");
            string forfatter = KonsollHjelper.LesTekst("Forfatter: ");
            int år = KonsollHjelper.LesTall("Utgivelsesår: ");
            int antall = KonsollHjelper.LesTall("Antall eksemplarer: ");

            system.BibliotekService.RegistrerBok(bokId, tittel, forfatter, år, antall);
            KonsollHjelper.VisSuksess("Boken ble registrert.");
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

        private void SeAlleBøker()
        {
            KonsollHjelper.VisOverskrift("Alle bøker i biblioteket");

            var bøker = system.BibliotekService.AlleBøker;
            if (bøker.Count == 0)
            {
                Console.WriteLine("Ingen bøker registrert.");
                return;
            }

            Console.WriteLine(String.Format("{0,-8} {1,-25} {2,-20} {3,-6} {4}",
                "ID", "Tittel", "Forfatter", "År", "Tilgj."));
            Console.WriteLine(new string('-', 75));

            foreach (var bok in bøker)
            {
                Console.WriteLine(String.Format("{0,-8} {1,-25} {2,-20} {3,-6} {4}/{5}",
                    bok.BokID, bok.Tittel, bok.Forfatter,
                    bok.Utgivelsesår, bok.TilgjengeligeEksemplarer, bok.AntallEksemplarer));
            }
        }

        private void SeAktiveLån()
        {
            KonsollHjelper.VisOverskrift("Aktive lån");

            var aktiveLån = system.BibliotekService.HentAlleAktiveLån();
            if (aktiveLån.Count == 0)
            {
                Console.WriteLine("Ingen aktive lån.");
                return;
            }

            Console.WriteLine($"Totalt {aktiveLån.Count} aktive lån:\n");
            foreach (var lån in aktiveLån)
            {
                Console.WriteLine($"  {lån.GetInfo()}");
            }
        }

        private void SeLånehistorikk()
        {
            KonsollHjelper.VisOverskrift("Lånehistorikk");

            var historikk = system.BibliotekService.HentLånehistorikk();
            if (historikk.Count == 0)
            {
                Console.WriteLine("Ingen lån registrert i systemet.");
                return;
            }

            Console.WriteLine($"Totalt {historikk.Count} lån i historikken:\n");

            // Vis aktive lån først
            var aktive = historikk.Where(l => l.ErAktiv).ToList();
            if (aktive.Count > 0)
            {
                KonsollHjelper.VisInfo("--- AKTIVE LÅN ---");
                foreach (var lån in aktive)
                {
                    Console.WriteLine($"  {lån.GetInfo()}");
                }
            }

            // Deretter returnerte lån
            var returnerte = historikk.Where(l => !l.ErAktiv).ToList();
            if (returnerte.Count > 0)
            {
                Console.WriteLine("\n--- RETURNERTE LÅN ---");
                foreach (var lån in returnerte)
                {
                    Console.WriteLine($"  {lån.GetInfo()}");
                }
            }
        }
    }
}
