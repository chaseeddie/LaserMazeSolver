using LaserMaze.Interfaces;
using LaserMaze.Models;
using System.Text.RegularExpressions;

namespace LaserMaze.Services
{
    public class LaserService : ILaserService
    {
        private readonly IMirrorService _mirrorService;

        public LaserService(IMirrorService mirrorService)
        {
            _mirrorService = mirrorService;
        }

        public LaserPoint GetLaserStartingPoint(string[] file)
        {
            LaserPoint startPoint = new LaserPoint();

            int laserSectionIndex = Array.IndexOf(file, "-1", Array.IndexOf(file, "-1") + 1);
            if (laserSectionIndex == -1 || laserSectionIndex + 1 >= file.Length)
            {
                throw new Exception("Laser starting position not found");
            }

            string laserSection = file[laserSectionIndex + 1];

            string[] laserDetails = laserSection.Split(","); // X , Yorientation
            var letter = Regex.Match(laserDetails[1], @"[A-Za-z]");
            if (letter.Success)
            {
                startPoint.X = int.Parse(laserDetails[0]);
                startPoint.Y = int.Parse(laserDetails[1].Substring(0, letter.Index));
                startPoint.Orientation = (char)laserDetails[1].Substring(letter.Index)[0];
            }
            else
            {
                throw new Exception("Invalid laser starting point");
            }


            if (laserDetails.Length < 2)
            {
                throw new Exception("Invalid laser starting point");
            }

            return startPoint;
        }

        public LaserPoint RunLaserPath(Maze maze)
        {
            var currentPoint = new LaserPoint()
            {
                X = maze.LaserStartPoint!.X,
                Y = maze.LaserStartPoint!.Y,
                Orientation = maze.LaserStartPoint.Orientation
            };

            while (true) // loop til we hit outside maze
            {
                LaserPoint previousPoint = new LaserPoint()
                {
                    Orientation = currentPoint.Orientation,
                    X = currentPoint.X,
                    Y = currentPoint.Y
                };
                currentPoint = MoveLaserOnePosition(currentPoint); // make first move

                if (LaserHasLeftTheMaze(maze, currentPoint))
                {
                    return previousPoint; // exit point before it left maze
                }
                else
                {
                    Mirror? mirrorOnPoint = _mirrorService.FindMirrorAtCurrentPoint(maze, currentPoint);
                    if (mirrorOnPoint != null)
                    {
                        // change orientation but wait til next loop to move
                        currentPoint = ReflectLaserOffMirror(currentPoint, mirrorOnPoint);
                    }
                }
            }
        }

        public Maze SetInitialLaserOrientation(Maze maze)
        {
            if (maze.LaserStartPoint?.Orientation == 'V')
            {
                if (maze.LaserStartPoint.Y == 0)
                {
                    maze.LaserStartPoint.Orientation = 'U'; // up
                }
                else if (maze.LaserStartPoint.Y == maze.Height - 1)
                {
                    maze.LaserStartPoint.Orientation = 'D'; // down
                }
                else
                {
                    throw new Exception("Invalid starting point based on Orientation");
                }
            }
            else if (maze.LaserStartPoint?.Orientation == 'H')
            {
                if (maze.LaserStartPoint.X == 0)
                {
                    maze.LaserStartPoint.Orientation = 'R'; // right
                }
                else if (maze.LaserStartPoint.X == maze.Width - 1)
                {
                    maze.LaserStartPoint.Orientation = 'L'; //left 
                }
                else
                {
                    throw new Exception("Invalid starting point based on Orientation");
                }
            }

            return maze;
        }

        public LaserPoint MoveLaserOnePosition(LaserPoint laser)
        {
            switch (laser.Orientation)
            {
                case 'U': laser.Y++; break; // move right 
                case 'D': laser.Y--; break; // down
                case 'L': laser.X--; break; // left
                case 'R': laser.X++; break; // right
                default:
                    throw new Exception("Invalid laser orientation");
            }
            return laser;
        }

        public LaserPoint ReflectLaserOffMirror(LaserPoint currentPoint, Mirror mirrorAtPoint)
        {
            if (mirrorAtPoint.Direction == 'R')
            {
                if (mirrorAtPoint.RightSideReflects)
                {
                    switch (currentPoint.Orientation)
                    {
                        case 'U': currentPoint.Orientation = 'R'; break;
                        case 'R': currentPoint.Orientation = 'D'; break;
                    }
                }

                if (mirrorAtPoint.LeftSideReflects)
                {
                    switch (currentPoint.Orientation)
                    {
                        case 'D': currentPoint.Orientation = 'L'; break;
                        case 'L': currentPoint.Orientation = 'U'; break;
                    }
                }
            }
            else // leans left
            {
                if (mirrorAtPoint.RightSideReflects)
                {
                    switch (currentPoint.Orientation)
                    {
                        case 'D': currentPoint.Orientation = 'R'; break;
                        case 'L': currentPoint.Orientation = 'U'; break;
                    }
                }

                if (mirrorAtPoint.LeftSideReflects)
                {
                    switch (currentPoint.Orientation)
                    {
                        case 'U': currentPoint.Orientation = 'L'; break;
                        case 'R': currentPoint.Orientation = 'D'; break;
                    }
                }
            }

            return currentPoint;
        }

        public bool LaserHasLeftTheMaze(Maze maze, LaserPoint currentPoint)
        {
            return (currentPoint.X < 0
                || currentPoint.Y < 0
                || currentPoint.X >= maze.Width
                || currentPoint.Y >= maze.Height);
        }
        public char SetLaserOutputOrientation(LaserPoint? point)
        {
            return (point?.Orientation == 'U' || point?.Orientation == 'D') ? 'V' : 'H'; // return to expected output
        }
    }
}
