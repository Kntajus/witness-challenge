using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Threading;

namespace Witness
{
    public class FileProcessor
    {
        private readonly PuzzleFinder _finder = new PuzzleFinder();
        private readonly string _outPath;
        private FileSystemWatcher _watcher;
        private DateTime _lastCreated = DateTime.UtcNow;

        public FileProcessor(string inPath, string outPath)
        {
            _outPath = outPath;
            _watcher = new FileSystemWatcher(inPath);
            _watcher.Created += Watcher_Created;
        }

        public void Start() => _watcher.EnableRaisingEvents = true;

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // FileSystemWatcher is pants and sometimes raises multiple events for the same file, skip if too soon
            if (DateTime.UtcNow - _lastCreated < TimeSpan.FromSeconds(1))
                return;
            _lastCreated = DateTime.UtcNow;

            try
            {
                var puzzle = GetPuzzle(e.FullPath);
                if (puzzle != null)
                {
                    puzzle.WriteToConsole();
                    var solution = puzzle.Solve();
                    var illustrator = new PuzzleIllustrator(puzzle);
                    var image = illustrator.Illustrate(solution);
                    image.SaveAsPng(Path.Combine(_outPath, "SOLVED_" + e.Name));
                }
                Console.WriteLine("============================");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private IPuzzle GetPuzzle(string imagePath)
        {
            IOException ioex = null;
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    return _finder.GetPuzzle(imagePath);
                }
                catch (IOException ex)
                {
                    ioex = ex;
                    // FileSystemWatcher being pants again - image is most likely still being written to disk, try again shortly
                    Thread.Sleep(100);
                }
            }
            throw ioex;
        }
    }
}
