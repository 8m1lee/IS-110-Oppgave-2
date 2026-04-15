using System;
using UniversitetSystem.Models;
using UniversitetSystem.Services;
using UniversitetSystem.Menyer;
using UniversitetSystem.Helpers;

namespace UniversitetSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Opprett hovedsystemet og legg inn testdata
            var system = new UniversitySystem();
            SeedData(system);

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("       UNIVERSITETS SYSTEM v2.0");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("[1] Logg inn (eksisterende bruker)");
                Console.WriteLine("[2] Registrer ny bruker");
                Console.WriteLine("[0] Avslutt");
                Console.WriteLine("════════════════════════════════════════");
                Console.Write("Velg: ");

                string valg = Console.ReadLine()?.Trim();

                switch (valg)
                {
                    case "1":
                        HåndterInnlogging(system);
                        break;
                    case "2":
                        HåndterRegistrering(system);
                        break;
                    case "0":
                        Console.WriteLine("Avslutter programmet...");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Ugyldig valg. Prøv igjen.");
                        KonsollHjelper.VentPåTast();
                        break;
                }
            }
        }

        /// <summary>
        /// Håndterer innlogging og ruter brukeren til riktig meny basert på rolle.
        /// </summary>
        static void HåndterInnlogging(UniversitySystem system)
        {
            KonsollHjelper.VisOverskrift("Innlogging");

            string brukernavn = KonsollHjelper.LesTekst("Brukernavn: ");

            Console.Write("Passord: ");
            string passord = LesPassord(); // Skjuler passord med *

            var bruker = system.InnloggingService.LoggInn(brukernavn, passord);

            if (bruker == null)
            {
                KonsollHjelper.VisFeil("Feil brukernavn eller passord.");
                KonsollHjelper.VentPåTast();
                return;
            }

            KonsollHjelper.VisSuksess($"\nVelkommen, {bruker.Navn}! (Rolle: {bruker.Rolle})");
            KonsollHjelper.VentPåTast();

            // Rut brukeren til riktig meny basert på rolle
            StartRollebasertMeny(system, bruker);
        }

        /// <summary>
        /// Ruter innlogget bruker til riktig meny basert på rolle.
        /// </summary>
        static void StartRollebasertMeny(UniversitySystem system, User bruker)
        {
            switch (bruker.Rolle)
            {
                case Rolle.Student:
                    var studentMeny = new StudentMeny(system, (Student)bruker);
                    studentMeny.Kjør();
                    break;

                case Rolle.Faglaerer:
                    var faglærerMeny = new FaglærerMeny(system, (Ansatt)bruker);
                    faglærerMeny.Kjør();
                    break;

                case Rolle.Bibliotekar:
                    var bibliotekarMeny = new BibliotekarMeny(system, (Ansatt)bruker);
                    bibliotekarMeny.Kjør();
                    break;

                default:
                    KonsollHjelper.VisFeil("Ukjent rolle. Kontakt administrator.");
                    break;
            }
        }

        /// <summary>
        /// Håndterer registrering av nye brukere.
        /// </summary>
        static void HåndterRegistrering(UniversitySystem system)
        {
            KonsollHjelper.VisOverskrift("Registrer ny bruker");

            Console.WriteLine("Velg brukertype:");
            Console.WriteLine("[1] Student");
            Console.WriteLine("[2] Utvekslingsstudent");
            Console.WriteLine("[3] Faglærer");
            Console.WriteLine("[4] Bibliotekar");
            Console.Write("Velg: ");

            string typeValg = Console.ReadLine()?.Trim();

            try
            {
                // Felles felt for alle brukertyper
                string navn = KonsollHjelper.LesTekst("Fullt navn: ");
                string email = KonsollHjelper.LesTekst("Email: ");
                string brukernavn = KonsollHjelper.LesTekst("Brukernavn: ");
                Console.Write("Passord: ");
                string passord = LesPassord();

                if (string.IsNullOrWhiteSpace(passord) || passord.Length < 3)
                {
                    KonsollHjelper.VisFeil("Passord må være minst 3 tegn.");
                    KonsollHjelper.VentPåTast();
                    return;
                }

                switch (typeValg)
                {
                    case "1":
                        var student = system.InnloggingService.RegistrerStudent(
                            navn, email, brukernavn, passord);
                        KonsollHjelper.VisSuksess(
                            $"\nStudent registrert! Din StudentID er: {student.StudentID}");
                        break;

                    case "2":
                        string hjemuni = KonsollHjelper.LesTekst("Hjemuniversitet: ");
                        string land = KonsollHjelper.LesTekst("Land: ");
                        DateTime fra = KonsollHjelper.LesDato("Periode fra (dd.MM.yyyy): ");
                        DateTime til = KonsollHjelper.LesDato("Periode til (dd.MM.yyyy): ");

                        if (til <= fra)
                        {
                            KonsollHjelper.VisFeil("Sluttdato må være etter startdato.");
                            KonsollHjelper.VentPåTast();
                            return;
                        }

                        var utvStudent = system.InnloggingService.RegistrerUtvekslingsStudent(
                            navn, email, brukernavn, passord, hjemuni, land, fra, til);
                        KonsollHjelper.VisSuksess(
                            $"\nUtvekslingsstudent registrert! Din StudentID er: {utvStudent.StudentID}");
                        break;

                    case "3":
                        string fagAvdeling = KonsollHjelper.LesTekst("Avdeling: ");
                        var fagLærer = system.InnloggingService.RegistrerAnsatt(
                            navn, email, brukernavn, passord,
                            "Faglærer", fagAvdeling, Rolle.Faglaerer);
                        KonsollHjelper.VisSuksess(
                            $"\nFaglærer registrert! Din AnsattID er: {fagLærer.AnsattID}");
                        break;

                    case "4":
                        var bib = system.InnloggingService.RegistrerAnsatt(
                            navn, email, brukernavn, passord,
                            "Bibliotekar", "Bibliotek", Rolle.Bibliotekar);
                        KonsollHjelper.VisSuksess(
                            $"\nBibliotekar registrert! Din AnsattID er: {bib.AnsattID}");
                        break;

                    default:
                        Console.WriteLine("Ugyldig valg.");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                KonsollHjelper.VisFeil(ex.Message);
            }
            catch (ArgumentException ex)
            {
                KonsollHjelper.VisFeil(ex.Message);
            }

            KonsollHjelper.VentPåTast();
        }

        /// <summary>
        /// Leser passord fra konsollen og viser * i stedet for tegnene.
        /// </summary>
        static string LesPassord()
        {
            string passord = "";
            ConsoleKeyInfo info;
            do
            {
                info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace && passord.Length > 0)
                {
                    passord = passord.Substring(0, passord.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(info.KeyChar))
                {
                    passord += info.KeyChar;
                    Console.Write("*");
                }
            } while (info.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return passord;
        }

        /// <summary>
        /// Legger til testdata slik at systemet har innhold ved oppstart.
        /// Alle testbrukere har passordet "123" for enkel testing.
        /// </summary>
        static void SeedData(UniversitySystem system)
        {
            // Studenter (brukernavn / passord: ola/123, kari/123, john/123)
            var student1 = new Student("S001", "Ola Nordmann", "ola@student.no", "ola", "123");
            var student2 = new Student("S002", "Kari Hansen", "kari@student.no", "kari", "123");
            var student3 = new UtvekslingStudent("S003", "John Smith", "john@student.no",
                "john", "123", "Harvard University", "USA",
                new DateTime(2025, 8, 1), new DateTime(2026, 6, 1));

            system.LeggTilStudent(student1);
            system.LeggTilStudent(student2);
            system.LeggTilStudent(student3);

            // Faglærer (brukernavn / passord: per/123)
            var ansatt1 = new Ansatt("A001", "Per Olsen", "per@universitetet.no",
                "per", "123", "Faglærer", "Informatikk", Rolle.Faglaerer);

            // Bibliotekar (brukernavn / passord: line/123)
            var ansatt2 = new Ansatt("A002", "Line Berg", "line@universitetet.no",
                "line", "123", "Bibliotekar", "Bibliotek", Rolle.Bibliotekar);

            system.LeggTilAnsatt(ansatt1);
            system.LeggTilAnsatt(ansatt2);

            // Kurs opprettet av faglærer Per Olsen
            system.KursService.OpprettKurs("DAT100", "Programmering", 10, 30, "A001");
            system.KursService.OpprettKurs("MAT101", "Matematikk 1", 10, 25, "A001");
            system.KursService.OpprettKurs("INF102", "Algoritmer", 10, 20, "A001");

            // Meld studenter på kurs
            system.KursService.MeldPåKurs("DAT100", student1);
            system.KursService.MeldPåKurs("DAT100", student2);
            system.KursService.MeldPåKurs("MAT101", student1);

            // Registrer bøker
            system.BibliotekService.RegistrerBok("B001", "C# Programming", "Anders Hejlsberg", 2020, 5);
            system.BibliotekService.RegistrerBok("B002", "Clean Code", "Robert Martin", 2008, 3);
            system.BibliotekService.RegistrerBok("B003", "Design Patterns", "Gang of Four", 1994, 2);

            // Registrer pensum
            system.KursService.RegistrerPensum("DAT100", "B001", "A001");
            system.KursService.RegistrerPensum("INF102", "B003", "A001");

            // Sett noen karakterer
            system.KursService.SettKarakter("DAT100", "S001", "B", "A001");

            // Lån ut bøker
            system.BibliotekService.LånBok("B001", student1);
            system.BibliotekService.LånBok("B002", ansatt1);
        }
    }
}
