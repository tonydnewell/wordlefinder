#
# Python implementation of the C# wordlefinder
# NOTE:
# - I'm not a python programmer so this is a crude translation of the C# code
# - There is no parallelism so it is a lot slower than the C# implementation
#   but still a lot faster than Matt Parker's implementation
#
# usage: python wordlefinder.py words-file.txt
#
import sys
import time

# bitmask to list of anagrams
wordList = {}
# list of all words
allWords = []
# found solutions
solutions = set()
number_of_searches = 0

def get_bitmasks():
    return list(wordList.keys())

def word_to_bitmask(word):
    bitmask = 0
    if word is None:
        return 0
    for c in word:
        charIndex = ord(c) - ord('a') + 1
        bitmask |= 1 << charIndex

    return bitmask

def read_file(path):
    file = open(path, 'r')
    while True:
        line = file.readline()
        if not line:
            break
        word = line.strip()
        if len(word) > 0:
            add_word(word)

def add_word(word):
    word = word.lower()
    if len(word) != 5:
        return
    for c in word:
        if c < 'a' or c > 'z':
            return
    if len(set(word)) != 5:
        return
    bitmask = word_to_bitmask(word)
    sameletters = wordList.get(bitmask)
    if sameletters is None:
        sameletters = []
        wordList[bitmask] = sameletters
    sameletters.append(word)
    allWords.append(word)

def words_with_same_letters(bitmask):
    return wordList.get(bitmask, [])

def print_stats():
    print("Number of words    : " + str(len(allWords)))
    print("Number of Bitmasks : " + str(len(wordList.keys())), flush=True)

def start_search():
    print("Starting search...")
    bitmasks = get_bitmasks()
    for i in range(len(bitmasks)):
        if i % 50 == 0:
            print("Iteration: "+str(i)+"    Searches: "
                +str(number_of_searches)+"  Solutions: "
                +str(len(solutions)), flush=True)
        found_words = set()
        search(bitmasks[i:], bitmasks[i], found_words)
    print("Finished search")

def search(original_bitmasks, word_bitmask, found_words):
    global number_of_searches

    found_words.add(word_bitmask)
    if len(found_words) == 5:
        add_found(found_words)
        number_of_searches += 1
        return
    bitmasks = create_filtered_copy(word_bitmask, original_bitmasks)
    if len(bitmasks) > 0:
        for i in range(len(bitmasks)):
            search(bitmasks, bitmasks[i], set(found_words))
    number_of_searches += 1

def create_filtered_copy(word_bitmask, original_bitmasks):
    ret = []
    for b in original_bitmasks:
        if b & word_bitmask == 0:
            ret.append(b)
    return ret

def add_found(found_words):
    solution_words = []
    for bitmask in found_words:
        anagrams = ""
        words = words_with_same_letters(bitmask)
        words.sort()
        for word in words:
            anagrams += word + " "
        solution_words.append(anagrams)

    solution_words.sort()
    solution = " || ".join(solution_words)

    if solution not in solutions:
        print("Solution: "+solution, flush=True)
        solutions.add(solution)

#
# Main
#
if __name__ == "__main__":
    read_file(sys.argv[1])
    print_stats()

    start_time = time.time()

    start_search()

    end_time = time.time()

    print("Time: "+str(end_time-start_time)+"s  Searches: "
        +str(number_of_searches)+"  Solutions: "
        +str(len(solutions)))


