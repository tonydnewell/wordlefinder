// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;


namespace wordlefinder
{
    public class WordleFinder
    {
        static void Main(string[] args)
        {
            var wordList = new WordList();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: wordlefinder words-file.txt");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Usage: wordlefinder words-file.txt");
                return;
            }

            wordList.ReadFile(args[0]);
            wordList.PrintStats();

            
            Finder finder = new(wordList, 5);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var timer = new Timer(
                s => Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds / 1000}s Searches: {finder.Searches} - Solutions: {finder.SolutionsCount}"),
                    null, 0, 5000);

            finder.Search();            

            timer.Dispose();
            stopwatch.Stop();

            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds / 1000}s Searches: {finder.Searches} - Solutions: {finder.SolutionsCount}");
        }
    }
}

