using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kincsvadaszok.Models
{
    internal class GameResult
    {
        public DateTime Date { get; set; }

        //jatekosok nevei
        public string P1Name { get; set; }
        public string P2Name { get; set; }

        //a jatekosok pontjai
        public int P1Score { get; set; }
        public int P2Score { get; set; }

        public string WinnerName { get; set; }

        public override string ToString()
        {
            return $"{Date:yyyy.MM.dd HH:mm} | Győztes {WinnerName.ToUpper()} ({P1Name}: vs {P2Name})"; // override a stilusos kiiratáshoz
        }
    }
}