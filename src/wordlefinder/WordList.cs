using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wordlefinder
{
    internal class WordList
    {
        private static readonly IList<string> EMPTY_LIST = new List<string>().AsReadOnly();

        private Dictionary<uint, List<string>> wordList = new Dictionary<uint, List<string>>();
        private List<string> allWords = new List<string>();
        private bool onlyUniqueLetters = true;

        public WordList()
        {
        }

        public WordList(bool onlyUniqueLetters)
        {
            this.onlyUniqueLetters = onlyUniqueLetters;
        }

        public uint[] GetBitmasks()
        {
            return wordList.Keys.ToArray();
        }

        public void ReadFile(string path)
        {
            foreach (string line in System.IO.File.ReadLines(path))
            {
                var word = line.Trim();
                if (word.Length > 0)
                {
                    AddWord(word);
                }
            }
        }

        private void AddWord(string word)
        {
            word = word.Trim().ToLower();

            // check length is 5 characters
            if (word.Length != 5)
            {
                return;
            }

            // check all alphabetic characters
            foreach (char c in word)
            {
                if (c < 'a' || c > 'z')
                {
                    return;
                }
            }

            // only words without duplicate letters
            if (onlyUniqueLetters && word.Distinct().Count() != 5)
            {
                return;
            }

            uint bitmask = WordToBitmask(word);

            if (!wordList.TryGetValue(bitmask, out List<string>? sameLetters))
            {
                sameLetters = new List<string>();
                wordList[bitmask] = sameLetters;
            }
            sameLetters.Add(word);
            allWords.Add(word);
        }

        public static uint WordToBitmask(string word)
        {
            uint bitmask = 0;
            
            if (word == null)
                return 0;

            foreach (char c in word)
            {
                bitmask |= (uint) (1 << c);
            }

            return bitmask;
        }

        public IList<string> WordsWithSameLetters(uint bitmask)
        {
            if (!wordList.TryGetValue(bitmask, out List<string>? sameLetters))
            {
                return EMPTY_LIST;
            }
            return sameLetters.AsReadOnly();
        }

        public void PrintStats()
        {
            Console.WriteLine($"Number of words    : {allWords.Count}");
            Console.WriteLine($"Number of Bitmasks : {wordList.Keys.Count}");
        }
    }
}
