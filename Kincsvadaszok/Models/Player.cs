namespace Kincsvadaszok.Models
{
    public class Player
    {
        public string Name {get; set;} // játékos neve 
        public int X {get;set;} // pozíció x koordinátája
        public int Y {get;set;} // pozíció Y koordinátája
        public int Score {get;set;} //pontszam

        public Player(string name, int startX, int startY)
        {
            Name=name;
            X = startX;
            Y=startY;
        }
    }
}