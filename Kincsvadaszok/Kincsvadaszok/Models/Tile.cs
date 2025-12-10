namespace Kincsvadaszok.Models
{
    public class Tile
    {
        public TileType Type {get; set;} // Minden mezőnek adunk egy típust
        public Tile(TileType type)
        {
            Type=type;
        }
    }
}