namespace LaserMaze.Models
{
    public class Mirror
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Direction { get; set; }
        public bool RightSideReflects { get; set; }
        public bool LeftSideReflects { get; set; }
    }
}
