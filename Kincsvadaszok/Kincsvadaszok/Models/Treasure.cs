public class Treasure
{
    public string Name { get; set; }
    public int Value { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Value} gold"; //rendes megjelenésért overrideoljuk a tostringet 
    }
}