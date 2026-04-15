using System;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer en utvekslingsstudent.
    /// Arver fra Student med ekstra informasjon om hjemuniversitet og oppholdsperiode.
    /// Arvehierarki: UtvekslingStudent -> Student -> User
    /// </summary>
    public class UtvekslingStudent : Student
    {
        public string Hjemuniversitet { get; set; }
        public string Land { get; set; }
        public DateTime PeriodeFra { get; set; }
        public DateTime PeriodeTil { get; set; }

        public UtvekslingStudent(string studentId, string navn, string email,
                                  string brukernavn, string passord,
                                  string hjemuniversitet, string land,
                                  DateTime periodeFra, DateTime periodeTil)
            : base(studentId, navn, email, brukernavn, passord)
        {
            Hjemuniversitet = hjemuniversitet;
            Land = land;
            PeriodeFra = periodeFra;
            PeriodeTil = periodeTil;
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, Hjemuniversitet: {Hjemuniversitet}, " +
                   $"Land: {Land}, Periode: {PeriodeFra:dd.MM.yyyy} - {PeriodeTil:dd.MM.yyyy}";
        }
    }
}
