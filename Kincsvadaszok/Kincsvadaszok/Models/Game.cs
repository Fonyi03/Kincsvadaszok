using System.Collections.Generic;
using System.Linq;

namespace Kincsvadaszok.Models
{
    public class Game
    {
        public Map GameMap {get;set;}
        public List<Player> Players {get;set;} 
        public int Currentplayerindex {get;set;} // ki jön éppen
        public int TurnCount{get;set;} //hányadik kör
        public int MaxCount{get;set;} // max körök száma

        public Game()
        {
            GameMap = new Map();
            Players = new List<Player>();
        }

        public void InitializeGame(List<string> playerNames, int maxTurns= 30)
        {
            GameMap.GenerateMap();
            MaxCount = maxTurns;
            TurnCount = 0;
            Currentplayerindex = 0;
            Players.Clear();

            if (playerNames.Count > 0) Players.Add(new Player(playerNames[0], 0, 0)); // 0,0 an kezdojatekos
            if (playerNames.Count > 1) Players.Add(new Player(playerNames[1], 4, 4)); // 4,4 en kezdojatekos
        }

        public void MovePlayer(int deltaX, int deltaY)
        {
            if (IsGameOver()) return; // jatek vege utan nem lephetunk

            Player current = Players[Currentplayerindex];
            int newX = current.X + deltaX;
            int newY = current.Y + deltaY;

            if (newX < 0 || newX >= GameMap.Width || newY < 0 || newY >= GameMap.Height) // pályán belül van-e?
                return;

            Tile targetTile = GameMap.GetTile(newX,newY);
            if (targetTile.Type == TileType.Obstacle) // megnezzuk hogy ahova lepunk ott fal van-e
                return; // ha igen nem lepunk 
            
            current.X = newX;
            current.Y = newY;

            if (targetTile.Type == TileType.Treasure) // kincs felvétele
            {
                current.Score++;
                targetTile.Type = TileType.Empty; // ha felvettuk a kincset akkor ures tile-ra cserelem 
            }

            TurnCount++;
            Currentplayerindex = (Currentplayerindex +1) %Players.Count; // körváltás
        }

        public bool IsGameOver()
        {
            if (TurnCount>=MaxCount) return true; // elfogytak a korok

            bool hasTreasureLeft = false;
            foreach (var tile in GameMap.Grid) // megnezzuk maradt e kincs a palyan 
            {
                if (tile.Type == TileType.Treasure)
                {
                    hasTreasureLeft = true;
                    break;
                }
            }
            return !hasTreasureLeft; // ha elgofyott akkor vege a jateknak 
        }

        public Player GetWinner()
        {
            if (!IsGameOver()) return null;
            return Players.OrderByDescending(p=> p.Score).FirstOrDefault();
        }
    }
}