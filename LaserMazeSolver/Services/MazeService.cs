using LaserMaze.Interfaces;
using LaserMaze.Models;
using System.Text.RegularExpressions;

namespace LaserMaze.Services
{
    public class MazeService : IMazeService
    {
        private readonly ILaserService _laserService;
        public MazeService(ILaserService laserService)
        {
            _laserService = laserService;
        }

        public Maze ReadMazeFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Maze file: {path} not found");
            }
            string[] file = File.ReadAllLines(path);

            var maze = GetMazeDimensions(file[0]); // first line contains dimensions

            maze.Mirrors = GetMirrors(file);
            maze.LaserStartPoint = _laserService.GetLaserStartingPoint(file);

            maze = _laserService.SetInitialLaserOrientation(maze);

            return maze;
        }

        public Maze GetMazeDimensions(string sizeString)
        {
            var maze = new Maze();
            string[] dimensions = sizeString.Split(",");
            maze.Width = int.Parse(dimensions[0]);
            maze.Height = int.Parse(dimensions[1]);
            maze.Mirrors = new List<Mirror>();

            return maze;
        }

        private static List<Mirror> GetMirrors(string[] file)
        {
            List<Mirror> mirrors = new List<Mirror>();
            int mirrorSectionStart = Array.IndexOf(file, "-1") + 1;
            int mirrorSectionEnd = Array.IndexOf(file, "-1", mirrorSectionStart);
            for (int i = mirrorSectionStart; i < mirrorSectionEnd; i++)
            {
                string mirrorLine = file[i];
                mirrors.Add(ParseMirrorLine(mirrorLine));
            }

            return mirrors;
        }

        private static Mirror ParseMirrorLine(string line)
        {
            var mirror = new Mirror();

            string[] mirrorDetails = line.Split(',');
            string letters = "";

            var letter = Regex.Match(mirrorDetails[1], @"[A-Za-z]");
            if (letter.Success)
            {
                mirror.X = int.Parse(mirrorDetails[0]);
                mirror.Y = int.Parse(mirrorDetails[1].Substring(0, letter.Index));
                letters = mirrorDetails[1].Substring(letter.Index);
            }
            else
            {
                throw new Exception("Invalid Mirror input");
            }

            if (letters.Length > 2)
            {
                throw new Exception("Invalid Mirror input");
            }
            char direction = (char)letters[0];

            mirror.Direction = (char)letters[0];
            mirror.RightSideReflects = letters.Length == 2 ? (char)letters[1] == 'R' : true;
            mirror.LeftSideReflects = letters.Length == 2 ? (char)letters[1] == 'L' : true;

            return mirror;
        }

        public Mirror? FindMirrorAtCurrentPoint(Maze maze, LaserPoint currentPoint)
        {
            return maze?.Mirrors?.FirstOrDefault(m => m.X == currentPoint?.X && m.Y == currentPoint?.Y);
        }
    }
}
