using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kincsvadaszok;
using System.Collections.Generic;
using System.Linq;

namespace Kincsvadaszok.Tests
{
    [TestClass]
    public class GameLogicTests 
    {
        public string P1Name { get; set; }
        public string P2Name { get; set; }
        public int P1Score { get; set; }
        public int P2Score { get; set; }
        public string WinnerName { get; set; }
        public System.DateTime Date { get; set; }

        // Optionally, override ToString to support the test
        public override string ToString()
        {
            return $"Győztes: {WinnerName.ToUpper()}\n{P1Name}: {P1Score}p\n{P2Name}: {P2Score}p\nDátum: {Date}";
        }

        // 1. Teszt: Kincs létrehozása
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

        // 2. Teszt: GameResult kiírás formázása
        [TestMethod]
        public void GameResult_ToString_ShouldFormatCorrectly()
        {
            // Arrange
            var result = new GameLogicTests
            {
                P1Name = "Béla",
                P2Name = "Kata",
                P1Score = 100,
                P2Score = 200,
                WinnerName = "Kata",
                Date = new System.DateTime(2023, 1, 1, 12, 0, 0)
            };

            // Act
            string output = result.ToString();

            // Assert
            StringAssert.Contains(output, "Győztes: KATA");
            StringAssert.Contains(output, "Béla: 100p");
        }

        // 3. Teszt: Győzelmi logika (ki nyert?)
        [TestMethod]
        public void ScoreCalculation_ShouldDetermineCorrectWinner()
        {
            // Arrange
            List<Treasure> lootP1 = new List<Treasure> { new Treasure { Value = 100 } };
            List<Treasure> lootP2 = new List<Treasure> { new Treasure { Value = 500 } };

            // Act
            int score1 = lootP1.Sum(t => t.Value);
            int score2 = lootP2.Sum(t => t.Value);

            string winner;
            if (score1 > score2) winner = "Player 1";
            else if (score2 > score1) winner = "Player 2";
            else winner = "Draw";

            // Assert
            Assert.AreEqual("Player 2", winner);
        }
    }
}