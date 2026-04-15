using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer en student ved universitetet.
    /// Arver fra User og har tilleggsfunksjonalitet for kurs og karakterer.
    /// </summary>
    public class Student : User
    {
        public string StudentID { get; set; }
        public List<Kurs> PåmeldteKurs { get; set; }

        // Dictionary som lagrer karakterer: KursKode -> Karakter
        public Dictionary<string, Karakter> Karakterer { get; set; }

        public Student(string studentId, string navn, string email,
                       string brukernavn, string passord)
            : base(navn, email, brukernavn, passord, Rolle.Student)
        {
            StudentID = studentId;
            PåmeldteKurs = new List<Kurs>();
            Karakterer = new Dictionary<string, Karakter>();
        }

        public override string GetUserId()
        {
            return StudentID;
        }

        public override string GetInfo()
        {
            return $"StudentID: {StudentID}, {base.GetInfo()}, Antall kurs: {PåmeldteKurs.Count}";
        }

        /// <summary>
        /// Legger til et kurs i studentens kursliste (intern metode, kalles fra Kurs).
        /// </summary>
        public void MeldPåKurs(Kurs kurs)
        {
            if (!PåmeldteKurs.Contains(kurs))
            {
                PåmeldteKurs.Add(kurs);
            }
        }

        /// <summary>
        /// Fjerner et kurs fra studentens kursliste.
        /// </summary>
        public void MeldAvKurs(Kurs kurs)
        {
            PåmeldteKurs.Remove(kurs);
            // Fjern eventuell karakter hvis studenten melder seg av
            if (Karakterer.ContainsKey(kurs.KursKode))
            {
                Karakterer.Remove(kurs.KursKode);
            }
        }

        /// <summary>
        /// Sjekker om studenten er påmeldt et gitt kurs.
        /// </summary>
        public bool ErPåmeldt(string kursKode)
        {
            return PåmeldteKurs.Any(k =>
                k.KursKode.Equals(kursKode, StringComparison.OrdinalIgnoreCase));
        }
    }
}
