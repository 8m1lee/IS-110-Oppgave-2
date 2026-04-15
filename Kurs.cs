using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer et universitetskurs.
    /// Inneholder informasjon om faglærer, påmeldte studenter,
    /// pensumlitteratur og karakterer.
    /// </summary>
    public class Kurs
    {
        public string KursKode { get; set; }
        public string KursNavn { get; set; }
        public int Studiepoeng { get; set; }
        public int MaksAntallPlasser { get; set; }
        public string FaglærerID { get; set; } // AnsattID til faglæreren

        private List<Student> påmeldteStudenter;
        public List<Student> PåmeldteStudenter => påmeldteStudenter;

        // Pensumliste: BokID-er knyttet til kurset
        private List<string> pensumBøker;
        public List<string> PensumBøker => pensumBøker;

        // Karakterer for studenter i kurset
        private Dictionary<string, Karakter> karakterer;
        public Dictionary<string, Karakter> Karakterer => karakterer;

        public Kurs(string kursKode, string kursNavn, int studiepoeng,
                    int maksAntallPlasser, string faglærerID)
        {
            if (string.IsNullOrWhiteSpace(kursKode))
                throw new ArgumentException("Kurskode kan ikke være tom.");
            if (string.IsNullOrWhiteSpace(kursNavn))
                throw new ArgumentException("Kursnavn kan ikke være tomt.");
            if (studiepoeng <= 0)
                throw new ArgumentException("Studiepoeng må være større enn 0.");
            if (maksAntallPlasser <= 0)
                throw new ArgumentException("Maks antall plasser må være større enn 0.");

            KursKode = kursKode;
            KursNavn = kursNavn;
            Studiepoeng = studiepoeng;
            MaksAntallPlasser = maksAntallPlasser;
            FaglærerID = faglærerID;
            påmeldteStudenter = new List<Student>();
            pensumBøker = new List<string>();
            karakterer = new Dictionary<string, Karakter>();
        }

        public bool HarLedigPlass()
        {
            return påmeldteStudenter.Count < MaksAntallPlasser;
        }

        /// <summary>
        /// Melder en student på kurset. Returnerer true hvis vellykket.
        /// Forhindrer duplikat-påmelding og sjekker kapasitet.
        /// </summary>
        public bool MeldPåStudent(Student student)
        {
            if (!HarLedigPlass()) return false;

            // Sjekk duplikat via StudentID for sikkerhet
            if (påmeldteStudenter.Any(s => s.StudentID == student.StudentID))
                return false;

            påmeldteStudenter.Add(student);
            student.MeldPåKurs(this);
            return true;
        }

        /// <summary>
        /// Melder en student av kurset.
        /// </summary>
        public bool MeldAvStudent(Student student)
        {
            var funnet = påmeldteStudenter.FirstOrDefault(s => s.StudentID == student.StudentID);
            if (funnet != null)
            {
                påmeldteStudenter.Remove(funnet);
                student.MeldAvKurs(this);
                // Fjern karakter om den finnes
                karakterer.Remove(student.StudentID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Setter karakter for en student i kurset.
        /// </summary>
        public bool SettKarakter(string studentId, string bokstavkarakter, string sattAv)
        {
            if (!Karakter.ErGyldigKarakter(bokstavkarakter))
                return false;

            if (!påmeldteStudenter.Any(s => s.StudentID == studentId))
                return false;

            var karakter = new Karakter(KursKode, studentId,
                bokstavkarakter.ToUpper(), sattAv);
            karakterer[studentId] = karakter;

            // Oppdater også studentens egen karakterliste
            var student = påmeldteStudenter.First(s => s.StudentID == studentId);
            student.Karakterer[KursKode] = karakter;
            return true;
        }

        /// <summary>
        /// Legger til en bok som pensum for kurset.
        /// </summary>
        public bool LeggTilPensum(string bokId)
        {
            if (pensumBøker.Contains(bokId, StringComparer.OrdinalIgnoreCase))
                return false;

            pensumBøker.Add(bokId);
            return true;
        }

        public int AntallPåmeldte()
        {
            return påmeldteStudenter.Count;
        }

        public string GetInfo()
        {
            return $"[{KursKode}] {KursNavn} - {Studiepoeng} stp, " +
                   $"Påmeldte: {AntallPåmeldte()}/{MaksAntallPlasser}, " +
                   $"Faglærer: {FaglærerID}";
        }
    }
}
