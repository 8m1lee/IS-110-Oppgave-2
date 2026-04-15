using System;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer et bokutlån i biblioteksystemet.
    /// Sporer utlånsdato, returdato og om lånet er aktivt.
    /// </summary>
    public class Lån
    {
        public Bok Bok { get; set; }
        public User Låntaker { get; set; }
        public DateTime LånDato { get; set; }
        public DateTime? ReturnertDato { get; set; }
        public bool ErAktiv { get; set; }

        public Lån(Bok bok, User låntaker)
        {
            Bok = bok ?? throw new ArgumentNullException(nameof(bok));
            Låntaker = låntaker ?? throw new ArgumentNullException(nameof(låntaker));
            LånDato = DateTime.Now;
            ReturnertDato = null;
            ErAktiv = true;
        }

        /// <summary>
        /// Markerer lånet som returnert og oppdaterer bokens tilgjengelighet.
        /// </summary>
        public void ReturnerBok()
        {
            if (ErAktiv)
            {
                ReturnertDato = DateTime.Now;
                ErAktiv = false;
                Bok.LeverInn();
            }
        }

        public string GetInfo()
        {
            string status = ErAktiv ? "Aktiv" : "Returnert";
            string returDato = ReturnertDato.HasValue
                ? ReturnertDato.Value.ToString("dd.MM.yyyy")
                : "Ikke returnert";

            return $"{Bok.Tittel} - Lånt av: {Låntaker.Navn} ({Låntaker.GetUserId()}) - " +
                   $"Lånt: {LånDato:dd.MM.yyyy} - Status: {status} - Returnert: {returDato}";
        }
    }
}
