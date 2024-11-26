using LaserMaze.Models;

namespace LaserMaze.Interfaces
{
    public interface ILaserService
    {
        public LaserPoint GetLaserStartingPoint(string[] file);
        public Maze SetInitialLaserOrientation(Maze maze);
        public LaserPoint RunLaserPath(Maze maze);
        public char SetLaserOutputOrientation(LaserPoint point);
    }
}
