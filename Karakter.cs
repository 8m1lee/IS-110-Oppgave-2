using System;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer en karakter gitt til en student i et kurs.
    /// Inneholder informasjon om hvem som satte karakteren og når.
    /// </summary>
    public class Karakter
    {
        public string KursKode { get; set; }
        public string StudentID { get; set; }
        public string Bokstavkarakter { get; set; } // A, B, C, D, E, F
        public string SattAv { get; set; }          // AnsattID til faglærer
        public DateTime DatoSatt { get; set; }

        public Karakter(string kursKode, string studentId,
                        string bokstavkarakter, string sattAv)
        {
            KursKode = kursKode;
            StudentID = studentId;
            Bokstavkarakter = bokstavkarakter;
            SattAv = sattAv;
            DatoSatt = DateTime.Now;
        }

        /// <summary>
        /// Validerer at karakteren er en gyldig bokstavkarakter (A-F).
        /// </summary>
        public static bool ErGyldigKarakter(string karakter)
        {
            string[] gyldige = { "A", "B", "C", "D", "E", "F" };
            return Array.Exists(gyldige, k =>
                k.Equals(karakter, StringComparison.OrdinalIgnoreCase));
        }

        public string GetInfo()
        {
            return $"Kurs: {KursKode}, Student: {StudentID}, " +
                   $"Karakter: {Bokstavkarakter}, Dato: {DatoSatt:dd.MM.yyyy}";
        }
    }
}
