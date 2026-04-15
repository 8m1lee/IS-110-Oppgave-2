using System;

namespace UniversitetSystem.Helpers
{
    /// <summary>
    /// Hjelpeklasse for konsolloperasjoner.
    /// Samler vanlige input/output-mønstre på ett sted for renere kode.
    /// </summary>
    public static class KonsollHjelper
    {
        /// <summary>
        /// Leser en ikke-tom streng fra brukeren. Spør på nytt hvis tomt.
        /// </summary>
        public static string LesTekst(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("Feltet kan ikke være tomt. Prøv igjen.");
            }
        }

        /// <summary>
        /// Leser et heltall fra brukeren med validering.
        /// </summary>
        public static int LesTall(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine()?.Trim(), out int tall) && tall > 0)
                    return tall;
                Console.WriteLine("Ugyldig tall. Skriv inn et positivt heltall.");
            }
        }

        /// <summary>
        /// Leser en dato fra brukeren (format: dd.MM.yyyy).
        /// </summary>
        public static DateTime LesDato(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine()?.Trim(), out DateTime dato))
                    return dato;
                Console.WriteLine("Ugyldig dato. Bruk format dd.MM.yyyy.");
            }
        }

        /// <summary>
        /// Viser en overskrift med dekorasjon.
        /// </summary>
        public static void VisOverskrift(string tittel)
        {
            Console.Clear();
            Console.WriteLine($"=== {tittel.ToUpper()} ===\n");
        }

        /// <summary>
        /// Venter på at brukeren trykker en tast.
        /// </summary>
        public static void VentPåTast()
        {
            Console.WriteLine("\nTrykk en tast for å fortsette...");
            Console.ReadKey();
        }

        /// <summary>
        /// Viser en feilmelding i rødt.
        /// </summary>
        public static void VisFeil(string melding)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FEIL: {melding}");
            Console.ResetColor();
        }

        /// <summary>
        /// Viser en suksessmelding i grønt.
        /// </summary>
        public static void VisSuksess(string melding)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(melding);
            Console.ResetColor();
        }

        /// <summary>
        /// Viser en informasjonsmelding i gult.
        /// </summary>
        public static void VisInfo(string melding)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(melding);
            Console.ResetColor();
        }
    }
}
