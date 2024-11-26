using LaserMaze.Models;

namespace LaserMaze.Interfaces
{
    public interface IMazeService
    {
        public Maze ReadMazeFile(string path);
        public Maze GetMazeDimensions(string sizeString);
    }
}
