# wordlefinder
Solving: Can you find five five-letter words that do not share any letters with each other E.g. five words with 25 unique letters between them

A quick hack to solve the problem:

This was posed in the episode of **A Problem Squared** podcast:
[https://aproblemsquared.libsyn.com/038-fldxt-in-wordle-and-improv-tact-hurdle](https://aproblemsquared.libsyn.com/038-fldxt-in-wordle-and-improv-tact-hurdle)

## Word lists

There are two word list

 - **words.txt** - dictionary file from [https://github.com/dwyl/english-words](https://github.com/dwyl/english-words)
 - **wordle-words-sorted.txt** - taken from the javascript from the Wordle game [https://www.nytimes.com/games/wordle/index.html](https://www.nytimes.com/games/wordle/index.html)

## Building and running
This solution was written in C# with .NET 6.0.  You can use the "dotnet" command to build and run, or use Visual Studio 2022.

The program takes one argument - the text file containing the words to use.

**crude python version** I've translated the C# into python. I'm not a python programmer so it could be improved. Also the python version
does not have any parallelism so is a **lot slower** but still faster than Matt Parker's solution. It completes in minutes not months.


## Algorithm overview

This a very high level overview of the algorithm:

Input:
 - each word is check that it is 5 letters long and has 5 distinct letters (no duplicate letters)
 - each word is converted into a bitmask - 26 bits for the letters of the alphabet used. The bitmasks are stored in an array. There is also a map from bitmask to all words with the same bitmask (all the anagrams of the word).

Searching:
 - a parallel for-loop is used to do several searches in parallel
 - searching involves removing bitmasks from the array (copy of array) that match the current word, and recursively doing that until 5 words are found or the array is empty
 - each bitmask in the array is used once as the starting point for a new search.  The array is trimmed so that only bitmasks following the current bitmask are included in the search (since earlier searches will have covered the earlier bitmasks).
 - the results are stored in a set (ConcurrentDictionary) so that duplicate results are ignored.

## Example runs

See **example-run-wordle.txt** and **example-run-words.txt**
