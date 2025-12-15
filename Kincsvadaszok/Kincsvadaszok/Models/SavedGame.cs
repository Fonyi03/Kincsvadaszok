using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kincsvadaszok.Models
{
    public class PointData
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class TreasureData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Treasure Item { get; set; }
    }

    public class SavedGame //teljes játékállás mentése
    {
        //játékosok adatai
        public string P1Name { get; set; }
        public string P2Name { get; set; }
        public int P1X { get; set; }
        public int P1Y { get; set; }
        public int P2X { get; set; }
        public int P2Y { get; set; }
        public int TotalSteps { get; set; }

        //Kinel vannak a kincsek 
        public List<Treasure> LootP1 { get; set; }
        public List<Treasure> LootP2 { get; set; }

        //jatek allapota
        public bool IsPlayer1Turn { get; set; }

        //palya elemeinek allapota
        public List<TreasureData> TreasureOnMap { get; set; }
        public List<PointData> ObstaclesOnMap { get; set; }

    }
}
