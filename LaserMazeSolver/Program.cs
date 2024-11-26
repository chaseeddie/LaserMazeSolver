using LaserMaze.Interfaces;
using LaserMaze.Models;
using LaserMaze.Services;
using Microsoft.Extensions.DependencyInjection;

public class LaserMazeSolver
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a valid .txt file");
            return;
        }
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ILaserService, LaserService>()
            .AddSingleton<IMazeService, MazeService>()
            .AddSingleton<IMirrorService, MirrorService>();

        try
        {
            var mazeService = serviceProvider.BuildServiceProvider().GetService<IMazeService>();
            var laserService = serviceProvider.BuildServiceProvider().GetService<ILaserService>();

            Maze maze = mazeService!.ReadMazeFile(args[0]);
            LaserPoint result = laserService!.RunLaserPath(maze);

            Console.WriteLine($"Maze Dimensions: {maze.Width}, {maze.Height}");
            Console.WriteLine($"Starting Point: {maze.LaserStartPoint?.X}, {maze.LaserStartPoint?.Y} {laserService.SetLaserOutputOrientation(maze!.LaserStartPoint!)}");
            Console.WriteLine($"Exit Point: {result.X}, {result.Y} {laserService.SetLaserOutputOrientation(result)}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error completing maze: {e.Message}");
        }
    }
}