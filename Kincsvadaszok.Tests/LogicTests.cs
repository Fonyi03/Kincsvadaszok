using Kincsvadaszok.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows.Input;

namespace Kincsvadaszok.Tests
{
    [TestClass]
    public class LogicTests
    {
        // --- 1. MODEL TESZT (Treasure) ---
        // Ez ellenőrzi, hogy a Kincs osztály helyesen tárolja-e az adatokat
        [TestMethod]
        public void Treasure_Creation_ShouldStoreCorrectValues()
        {
            // Arrange
            string expectedName = "Arany Serleg";
            int expectedValue = 500;

            // Act
            Treasure t = new Treasure { Name = expectedName, Value = expectedValue };

            // Assert
            Assert.AreEqual(expectedName, t.Name);
            Assert.AreEqual(expectedValue, t.Value);
        }

        // --- 2. LOGIKA TESZTEK (GameLogic) ---

        // Mozgás tesztelése (W gomb -> Y csökken)
        [TestMethod]
        public void TestMovement_Up()
        {
            var result = GameLogic.CalculateNewPosition(5, 5, Key.W, 10);

            Assert.AreEqual(5, result.x);
            Assert.AreEqual(4, result.y);
        }

        // Pálya széle teszt (Bal felső sarokból nem mehetünk ki)
        [TestMethod]
        public void TestMovement_Boundary_Top()
        {
            var result = GameLogic.CalculateNewPosition(0, 0, Key.W, 10);

            // Maradni kell 0-n
            Assert.AreEqual(0, result.y);
        }

        // Pálya széle teszt (Jobb alsó sarokból nem mehetünk ki)
        [TestMethod]
        public void TestMovement_Boundary_Right()
        {
            var result = GameLogic.CalculateNewPosition(9, 9, Key.D, 10);

            // Maradni kell 9-en
            Assert.AreEqual(9, result.x);
        }

        // Falnak ütközés tesztelése
        [TestMethod]
        public void TestCollision_WithWall()
        {
            HashSet<(int, int)> walls = new HashSet<(int, int)>();
            walls.Add((1, 1)); // Fal van az 1,1-en

            // Megpróbálunk rálépni az 1,1-re. Ez NEM lehet érvényes.
            // Fontos: Itt a GameLogic-ban nagybetűs IsValidMove-nak kell lennie!
            bool isValid = GameLogic.isValidMove(1, 1, walls, 5, 5);

            Assert.IsFalse(isValid);
        }

        // Másik játékosnak ütközés tesztelése
        [TestMethod]
        public void TestCollision_WithOtherPlayer()
        {
            HashSet<(int, int)> walls = new HashSet<(int, int)>();

            // A másik játékos a 2,2-n áll, mi oda akarunk lépni
            bool isValid = GameLogic.isValidMove(2, 2, walls, 2, 2);

            Assert.IsFalse(isValid);
        }

        // Győztes tesztelése (Sima pontelőny)
        [TestMethod]
        public void TestWinner_Player1()
        {
            string winner = GameLogic.GetWinner(100, 50, "P1", "P2", false);
            Assert.AreEqual("P1", winner);
        }

        // Döntetlen tesztelése (Lépéslimit)
        [TestMethod]
        public void TestWinner_DrawBySteps()
        {
            string winner = GameLogic.GetWinner(100, 50, "P1", "P2", true);

            // Ez feltételezi, hogy a GameLogic-ban kijavítottad nagy 'L'-re a szöveget:
            Assert.AreEqual("Döntetlen (Lépéslimit)", winner);
        }
    }
}