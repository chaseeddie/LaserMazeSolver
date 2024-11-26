namespace LaserMaze.Models
{
    public class Maze
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public List<Mirror>? Mirrors { get; set; }
        public LaserPoint? LaserStartPoint { get; set; }
    }
}
