using LaserMaze.Interfaces;
using LaserMaze.Models;
using System.Text.RegularExpressions;

namespace LaserMaze.Services
{
    public class MirrorService : IMirrorService
    {
        public static List<Mirror> GetMirrors(string[] file)
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

        public static Mirror ParseMirrorLine(string line)
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
            return maze.Mirrors?.FirstOrDefault(m => m.X == currentPoint?.X && m.Y == currentPoint?.Y);
        }
    }
}
