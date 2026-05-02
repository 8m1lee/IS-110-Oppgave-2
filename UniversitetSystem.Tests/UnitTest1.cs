using Xunit;
using UniversitetSystem;
using UniversitetSystem.Models;
using UniversitetSystem.Services;

namespace UniversitetSystem.Services
public class UnitTest1
{
     [Fact]
        public void RegistrerBok_NyBok_BlirLagtTilOgKanFinnesIgjen()
        {
            // Arrange — lag et tomt bibliotek
            var bibliotek = new BibliotekService();

            // Act — registrer en bok og finn den igjen\", "Ola Forfatter", 2024, 3);
            var funnetBok = bibliotek.FinnBok("B100");

            // Assert — boken skal finnes med riktige verdier
            Assert.NotNull(funnetBok);
            Assert.Equal("Test Bok", funnetBok.Tittel);
            Assert.Equal("Ola Forfatter", funnetBok.Forfatter);
            Assert.Equal(3, funnetBok.AntallEksemplarer);
            Assert.Equal(3, funnetBok.TilgjengeligeEksemplarer);
        }

        // ----------------------------------------------------------------
        // TEST 2: Duplikat BokID skal kaste exception
        // ----------------------------------------------------------------
        [Fact]
        public void RegistrerBok_DuplikatBokId_KasterInvalidOperationException()
        {
            // Arrange — registrer en bok først
            var bibliotek = new BibliotekService();
            bibliotek.RegistrerBok("B200", "Bok 1", "Forfatter A", 2020, 1);

            // Act + Assert — å registrere en bok med samme ID skal feile
            var exception = Assert.Throws<InvalidOperationException>(() =>
                bibliotek.RegistrerBok("B200", "Bok 2", "Forfatter B", 2021, 2));

            Assert.Contains("B200", exception.Message);
        }

        // ----------------------------------------------------------------
        // TEST 3: Studenter blir påmeldt et kurs
        // ----------------------------------------------------------------
        [Fact]
        public void MeldPåKurs_StudentMeldesPå_StudentenErPåmeldtKurset()
        {
            // Arrange — opprett et kurs og en student
            var kursService = new KursService();
            var student = new Student("S999", "Test Student",
                "test@student.no", "test", "passord");
            kursService.OpprettKurs("TEST100", "Testkurs", 10, 30, "A001");

            // Act — meld studenten på kurset
            kursService.MeldPåKurs("TEST100", student);

            // Assert — studenten skal være påmeldt
            Assert.True(student.ErPåmeldt("TEST100"));
            Assert.Single(student.PåmeldteKurs);
        }

        // ----------------------------------------------------------------
        // TEST 4: Lån av bok reduserer antall tilgjengelige eksemplarer
        // ----------------------------------------------------------------
        [Fact]
        public void LånBok_ReduserTilgjengeligeEksemplarer_OgRegistrerAktivtLån()
        {
            // Arrange — bibliotek med en bok (2 eksemplarer) og en student
            var bibliotek = new BibliotekService();
            bibliotek.RegistrerBok("B300", "Lånebok", "Forfatter", 2024, 2);
            var student = new Student("S888", "Lånetaker",
                "l@t.no", "lt", "pass");

            // Act — lån ut én bok
            bibliotek.LånBok("B300", student);

            // Assert — tilgjengelig er nå 1, og det skal finnes ett aktivt lån
            var bok = bibliotek.FinnBok("B300");
            Assert.Equal(1, bok.TilgjengeligeEksemplarer);
            Assert.Equal(2, bok.AntallEksemplarer);

            var aktiveLån = bibliotek.HentAktiveLån("S888");
            Assert.Single(aktiveLån);
            Assert.True(aktiveLån[0].ErAktiv);
            Assert.Equal("B300", aktiveLån[0].Bok.BokID);
        }
    }

