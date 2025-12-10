using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kincsvadaszok.Models;
using System.Collections.Generic;

namespace Kincsvadaszok.Tests
{
    [TestClass]
    public class GameTests
    {
        private Game _game;

        // ez a metódus minden egyes teszt előtt lefut, hogy tiszta lappal induljunk
        [TestInitialize]
        public void Setup()
        {
            _game = new Game();
            _game.InitializeGame(new List<string> { "TesztJatekos1", "TesztJatekos2" });

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    _game.GameMap.Grid[x, y].Type = TileType.Empty;
                }
            }

            _game.GameMap.Grid[3, 3].Type = TileType.Treasure;
        }
        [TestMethod]
        public void MovePlayer_ValidMove_UpdatesPosition()
        {
            // Teszt: Ha a játékos jobbra lép, változik az X koordináta?
            
            // Arrange (Előkészítés): Játékos1 a (0,0)-n van
            _game.Currentplayerindex = 0; 
            
            // Act (Cselekvés): Lépés jobbra (x + 1)
            _game.MovePlayer(1, 0); 

            // Assert (Ellenőrzés)
            Assert.AreEqual(1, _game.Players[0].X); // Most az 1-es helyen kell lennie
            Assert.AreEqual(0, _game.Players[0].Y);
        }

        [TestMethod]
        public void MovePlayer_IntoObstacle_DoesNotMove()
        {
            // Teszt: Ha falnak megy, marad-e a helyén?

            // Arrange: Teszünk egy akadályt a (1,0) helyre
            _game.GameMap.Grid[1, 0].Type = TileType.Obstacle;
            _game.Currentplayerindex = 0; // Játékos a (0,0)-n

            // Act: Megpróbálunk rálépni a falra
            _game.MovePlayer(1, 0);

            // Assert: A pozíciónak NEM szabad változnia
            Assert.AreEqual(0, _game.Players[0].X);
        }

        [TestMethod]
        public void MovePlayer_CollectsTreasure_IncreasesScore()
        {
            // Teszt: Ha kincsre lép, kap-e pontot?

            // Arrange: Teszünk egy kincset a (0,1) helyre (lefelé)
            _game.GameMap.Grid[0, 1].Type = TileType.Treasure;
            _game.Currentplayerindex = 0;

            // Act: Lépés lefelé
            _game.MovePlayer(0, 1);

            // Assert
            Assert.AreEqual(1, _game.Players[0].Score); // Pontszám nőtt?
            Assert.AreEqual(TileType.Empty, _game.GameMap.Grid[0, 1].Type); // A kincs eltűnt?
        }

        [TestMethod]
        public void TurnSystem_SwitchPlayer_AfterMove()
        {
            // Teszt: Lépés után a másik játékos jön?
            
            _game.Currentplayerindex = 0; // 1. játékos
            _game.MovePlayer(0, 1); // Lép egyet
            
            Assert.AreEqual(1, _game.Currentplayerindex); // Most a 2. játékosnak (index 1) kell jönnie
        }

        [TestMethod]
        public void GameOver_MaxTurnsReached_ReturnsTrue()
        {
            // Teszt: Ha elértük a max kört, vége a játéknak?

            _game.MaxCount = 5;
            _game.TurnCount = 5; // Beállítjuk a végső körre
            
            Assert.IsTrue(_game.IsGameOver());
        }
    }
}