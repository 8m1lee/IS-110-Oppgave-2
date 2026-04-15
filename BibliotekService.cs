using System;
using System.Collections.Generic;
using System.Linq;
using UniversitetSystem.Models;

namespace UniversitetSystem.Services
{
    /// <summary>
    /// Tjeneste for bibliotekfunksjoner: bokregistrering, utlån, retur,
    /// søk, og lånehistorikk.
    /// </summary>
    public class BibliotekService
    {
        private readonly List<Bok> alleBøker;
        private readonly List<Lån> alleLån; // Inneholder både aktive og historiske lån

        public BibliotekService()
        {
            alleBøker = new List<Bok>();
            alleLån = new List<Lån>();
        }

        public List<Bok> AlleBøker => alleBøker;
        public List<Lån> AlleLån => alleLån;

        /// <summary>
        /// Registrerer en ny bok i biblioteket.
        /// </summary>
        public Bok RegistrerBok(string bokId, string tittel, string forfatter,
                                 int utgivelsesår, int antall)
        {
            if (alleBøker.Any(b => b.BokID.Equals(bokId, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    $"En bok med ID '{bokId}' er allerede registrert.");

            var bok = new Bok(bokId, tittel, forfatter, utgivelsesår, antall);
            alleBøker.Add(bok);
            Console.WriteLine($"Bok registrert: {bok.GetInfo()}");
            return bok;
        }

        /// <summary>
        /// Søker etter bøker basert på tittel, forfatter eller BokID.
        /// </summary>
        public List<Bok> SøkBok(string søkeord)
        {
            if (string.IsNullOrWhiteSpace(søkeord))
                return new List<Bok>();

            string søk = søkeord.ToLower();
            return alleBøker.Where(b =>
                b.BokID.ToLower().Contains(søk) ||
                b.Tittel.ToLower().Contains(søk) ||
                b.Forfatter.ToLower().Contains(søk))
                .ToList();
        }

        /// <summary>
        /// Finner en bok basert på BokID.
        /// </summary>
        public Bok FinnBok(string bokId)
        {
            return alleBøker.FirstOrDefault(b =>
                b.BokID.Equals(bokId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Låner ut en bok til en bruker.
        /// Sjekker at boken finnes, er tilgjengelig, og at brukeren
        /// ikke allerede har et aktivt lån på samme bok.
        /// </summary>
        public Lån LånBok(string bokId, User bruker)
        {
            var bok = FinnBok(bokId);
            if (bok == null)
                throw new KeyNotFoundException($"Bok med ID '{bokId}' finnes ikke.");

            if (!bok.ErTilgjengelig())
                throw new InvalidOperationException(
                    $"Boken '{bok.Tittel}' har ingen tilgjengelige eksemplarer.");

            // Sjekk om brukeren allerede har et aktivt lån på denne boken
            bool harAktivtLån = alleLån.Any(l =>
                l.ErAktiv &&
                l.Bok.BokID.Equals(bokId, StringComparison.OrdinalIgnoreCase) &&
                l.Låntaker.GetUserId() == bruker.GetUserId());

            if (harAktivtLån)
                throw new InvalidOperationException(
                    $"Brukeren har allerede lånt boken '{bok.Tittel}'.");

            bok.LånUt();
            var lån = new Lån(bok, bruker);
            alleLån.Add(lån);
            Console.WriteLine($"Bok lånt: {bok.Tittel} til {bruker.Navn}");
            return lån;
        }

        /// <summary>
        /// Returnerer en bok fra en bruker.
        /// </summary>
        public bool ReturnerBok(string bokId, User bruker)
        {
            var lån = alleLån.FirstOrDefault(l =>
                l.ErAktiv &&
                l.Bok.BokID.Equals(bokId, StringComparison.OrdinalIgnoreCase) &&
                l.Låntaker.GetUserId() == bruker.GetUserId());

            if (lån == null)
                throw new KeyNotFoundException(
                    $"Finner ikke aktivt lån for bok '{bokId}' for bruker '{bruker.Navn}'.");

            lån.ReturnerBok();
            Console.WriteLine($"Bok returnert: {lån.Bok.Tittel} fra {bruker.Navn}");
            return true;
        }

        /// <summary>
        /// Henter aktive lån for en bestemt bruker.
        /// </summary>
        public List<Lån> HentAktiveLån(string brukerId)
        {
            return alleLån.Where(l =>
                l.ErAktiv && l.Låntaker.GetUserId() == brukerId).ToList();
        }

        /// <summary>
        /// Henter alle aktive lån i systemet.
        /// </summary>
        public List<Lån> HentAlleAktiveLån()
        {
            return alleLån.Where(l => l.ErAktiv).ToList();
        }

        /// <summary>
        /// Henter full lånehistorikk (inkludert returnerte lån).
        /// </summary>
        public List<Lån> HentLånehistorikk()
        {
            return alleLån.OrderByDescending(l => l.LånDato).ToList();
        }
    }
}
