using System.Collections.Concurrent;
using System.Text;

namespace wordlefinder
{
    internal class Finder
    {
        private static readonly uint[] EMPTY_ARRAY = Array.Empty<uint>();

        private readonly WordList wordList;
        private readonly int numberToFind;
        private readonly ConcurrentDictionary<string, byte> solutionHashs = new();
        private long searches = 0;

        public Finder(WordList wordList, int numberToFind)
        {
            this.wordList = wordList;
            this.numberToFind = numberToFind;
        }

        public long Searches
        {
            get { return Interlocked.Read(ref searches); }
        }

        public int SolutionsCount
        {
            get { return solutionHashs.Count; }
        }

        public void Search()
        {
            Console.WriteLine("Starting search...");
            uint[] bitmasks = wordList.GetBitmasks();
            var rangePartitioner = Partitioner.Create(0, bitmasks.Length);
            Parallel.ForEach(rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    //Console.WriteLine("STARTING FROM: " + i);
                    HashSet<uint> foundWords = new();
                    Search(CreateCopy(bitmasks, i), bitmasks[i], foundWords);
                }
            });
            Console.WriteLine("Finished search");
        }

        public void Search(uint[] originalBitmasks, uint wordBitmask, HashSet<uint> foundWords)
        {
            //Console.WriteLine($"  array length={originalBitmasks.Length}");
            foundWords.Add(wordBitmask);
            if (foundWords.Count == numberToFind)
            {
                AddFound(foundWords);
                Interlocked.Increment(ref searches);
                return;
            }

            uint[] bitmasks = CreateFilteredCopy(wordBitmask, originalBitmasks);
            if (bitmasks.Length > 0)
            {
                for (int i = 0; i < bitmasks.Length; i++)
                {
                    uint nextWordBitmask = bitmasks[i];
                    Search(bitmasks, nextWordBitmask, new HashSet<uint>(foundWords));
                }
            }

            Interlocked.Increment(ref searches);
        }

        private void AddFound(HashSet<uint> foundWords)
        {
            string[] solutionWords = new string[foundWords.Count];

            int i = 0;
            foreach (uint bitmask in foundWords)
            {
                StringBuilder sb = new StringBuilder();
                IList<string> words = wordList.WordsWithSameLetters(bitmask).OrderBy(s => s).ToList();
                foreach (string word in words)
                {
                    sb.Append(word + " ");
                }
                solutionWords[i] = sb.ToString();
                ++i;
            }

            Array.Sort(solutionWords);
            string solution = String.Join(" || ", solutionWords);
          
            if (solutionHashs.TryAdd(solution, 0))
            {
               Console.WriteLine("Solution: " + solution);
            }

        }

        private static uint[] CreateFilteredCopy(uint bitmask, uint[] bitmasks)
        {
            int size = bitmasks.Length;
            for (int i = 0; i < bitmasks.Length; i++)
            {
                if ((bitmasks[i] & bitmask) != 0)
                {
                    --size;
                }
            }

            uint[] ret = new uint[size];
            int j = 0;
            for (int i = 0; i < bitmasks.Length; i++)
            {
                if ((bitmasks[i] & bitmask) == 0)
                {
                    ret[j] = bitmasks[i];
                    ++j;
                }
            }

            return ret;
        }
        private static uint[] CreateCopy(uint[] bitmasks, int start)
        {
            if (bitmasks.Length <= start)
            {
                return EMPTY_ARRAY;
            }

            uint[] ret = new uint[bitmasks.Length - start];
            int j = 0;
            for (int i = start; i < bitmasks.Length; i++)
            {
                ret[j] = bitmasks[i];
                ++j;
            }

            return ret;
        }
    }
}
