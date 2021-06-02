using System;

namespace Witness
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var screenshotFolderPath = args[0];
            var outFolderPath = args[1];
            Console.WriteLine("Monitoring: " + screenshotFolderPath);
            Console.WriteLine("Solutions saved to: " + outFolderPath);
            Console.WriteLine("Press any key to exit.");

            var processor = new FileProcessor(screenshotFolderPath, outFolderPath);
            processor.Start();

            Console.ReadKey();
        }
    }
}
