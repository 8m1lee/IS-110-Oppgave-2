using System;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer en bok i universitetets bibliotek.
    /// Holder styr på totalt antall eksemplarer og tilgjengelighet.
    /// </summary>
    public class Bok
    {
        public string BokID { get; set; }
        public string Tittel { get; set; }
        public string Forfatter { get; set; }
        public int Utgivelsesår { get; set; }
        public int AntallEksemplarer { get; set; }
        public int TilgjengeligeEksemplarer { get; set; }

        public Bok(string bokId, string tittel, string forfatter,
                   int utgivelsesår, int antallEksemplarer)
        {
            if (string.IsNullOrWhiteSpace(bokId))
                throw new ArgumentException("BokID kan ikke være tom.");
            if (string.IsNullOrWhiteSpace(tittel))
                throw new ArgumentException("Tittel kan ikke være tom.");
            if (antallEksemplarer < 1)
                throw new ArgumentException("Antall eksemplarer må være minst 1.");

            BokID = bokId;
            Tittel = tittel;
            Forfatter = forfatter;
            Utgivelsesår = utgivelsesår;
            AntallEksemplarer = antallEksemplarer;
            TilgjengeligeEksemplarer = antallEksemplarer;
        }

        public bool ErTilgjengelig()
        {
            return TilgjengeligeEksemplarer > 0;
        }

        public bool LånUt()
        {
            if (!ErTilgjengelig()) return false;
            TilgjengeligeEksemplarer--;
            return true;
        }

        public void LeverInn()
        {
            if (TilgjengeligeEksemplarer < AntallEksemplarer)
            {
                TilgjengeligeEksemplarer++;
            }
        }

        public string GetInfo()
        {
            return $"[{BokID}] {Tittel} av {Forfatter} ({Utgivelsesår}) - " +
                   $"Tilgjengelig: {TilgjengeligeEksemplarer}/{AntallEksemplarer}";
        }
    }
}
