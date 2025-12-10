using System;

namespace Kincsvadaszok.Models
{
    public class Map
    {
        public Tile[,] Grid {get;set;} // rács ami az 5x5-ös pályát tárol
        public int Width{get;} = 5;
        public int Height{get;} = 5;

        public Map()
        {
            Grid = new Tile[Width,Height];
        }

        public void GenerateMap()
        {

            //először minden cella üres
            for(int x = 0; x < Width; x++)
            {
                for(int y=0; y < Height; y++)
                {
                    Grid[x,y]=new Tile(TileType.Empty);
                }
            }

            Random rand = new Random();

            PlaceRandomItems(rand, TileType.Treasure, 10); // kincs elhelyezése

            PlaceRandomItems(rand, TileType.Obstacle, 5); // akadály elhelyezése


            
        }


        private void PlaceRandomItems(Random rand, TileType type, int count) // segédfüggvény a logikához
        {
            int placed = 0;
            while (placed < count)
            {
                int x = rand.Next(Width);
                int y = rand.Next(Height);

                bool isStartZone = (x == 0 && y==0) || (x == Width -1 && y==Height-1); // 0,0 és 4,4 koordináták a kezdőmezők, ezek kimaradnak

                if (Grid[x,y].Type == TileType.Empty && !isStartZone) // csak akkor van lerakva ha üres és nem kezdőhely
                {
                    Grid[x,y].Type = type;
                    placed++;
                }


            }
        }

        public Tile GetTile(int x, int y) // adott helyen mi van <--ehhez lekérdezés
        {
            if (x>=0 && x<Width && y>=0 && y < Height)
            {
                return Grid[x,y];
            }
            return null;
        }
    }
}