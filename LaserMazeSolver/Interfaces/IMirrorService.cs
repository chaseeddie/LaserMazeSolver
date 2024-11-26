using LaserMaze.Models;

namespace LaserMaze.Interfaces
{
    public interface IMirrorService
    {
        public Mirror? FindMirrorAtCurrentPoint(Maze maze, LaserPoint currentPoint);
    }
}
