using System;

namespace UniversitetSystem.Models
{
    /// <summary>
    /// Representerer en ansatt ved universitetet.
    /// Kan ha rollen Faglaerer eller Bibliotekar.
    /// </summary>
    public class Ansatt : User
    {
        public string AnsattID { get; set; }
        public string Stilling { get; set; }
        public string Avdeling { get; set; }

        public Ansatt(string ansattId, string navn, string email,
                      string brukernavn, string passord,
                      string stilling, string avdeling, Rolle rolle)
            : base(navn, email, brukernavn, passord, rolle)
        {
            AnsattID = ansattId;
            Stilling = stilling;
            Avdeling = avdeling;
        }

        public override string GetUserId()
        {
            return AnsattID;
        }

        public override string GetInfo()
        {
            return $"AnsattID: {AnsattID}, {base.GetInfo()}, " +
                   $"Stilling: {Stilling}, Avdeling: {Avdeling}";
        }
    }
}
