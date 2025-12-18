using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kincsvadaszok.Models
{
    public class GameLogic
    {
        public static (int x, int y) CalculateNewPosition(int currentX, int currentY, Key key, int mapSize) //kiszamolja hova lepne a jatekos
        {
            int newX = currentY;
            int newY = currentY;

            switch (key)
            {
                case Key.W:
                case Key.Up:
                    if (currentY > 0) newY--;
                    break;
                case Key.S:
                case Key.Down:
                    if (currentY < mapSize - 1) newY++;
                    break;
                case Key.A:
                case Key.Left:
                    if (currentX > 0) newX--;
                    break;
                case Key.D:
                case Key.Right:
                    if (currentX < mapSize - 1) newX++;
                    break;
            }
            return (newX, newY);
        }

        public static bool isValidMove(int newX, int newY, HashSet<(int,int)> obstacles, int otherX, int otherY)
        {
            if (obstacles.Contains((newX, newY))) return false; // falnak ütközés

            if (newX == otherX && newY == otherY) return false; // masik jatekosnak utkozes

            return true;
        }

        public static string GetWinner(int p1Score, int p2Score, string p1Name, string p2Name, bool isDrawBySteps)
        {
            if (isDrawBySteps) return "Döntetlen (Lépéslimit)";
            if (p1Score > p2Score) return p1Name;
            if (p2Score > p1Score) return p2Name;
            return "Döntetlen";
        }
    }
}
