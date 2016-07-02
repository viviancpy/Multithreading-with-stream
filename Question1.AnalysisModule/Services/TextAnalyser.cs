using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Question1.AnalysisModule.Models;

namespace Question1.AnalysisModule.Services
{
    /// <summary>
    /// TextAnalyser is the class which contains the logics about creating analysis report to users
    /// </summary>
    public class TextAnalyser : ITextAnalyser
    {
        private const char WordSplitter = ' ';
        private const int LargestWordListCount = 5;
        private const int SmallestWordListCount = 5;
        private const int PopularWordListCount = 10;

        // Fields for characters
        private int _numberOfCharacters;
        private readonly Dictionary<char, int> _charFrequency = new Dictionary<char, int>();

        // Fields for words
        private string _lastWord = string.Empty;
        private int _numberOfWords;
        private readonly Dictionary<string, int> _wordFrequency = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _popularWordFrequency = new Dictionary<string, int>();
        private readonly List<string> _largestWords = new List<string>();
        private readonly List<string> _smallestWords = new List<string>();

        private readonly object _analyserLock = new object();

        public async Task<AnalyzeResult> AnalyzeText()
        {
            // Get the analyze result if there is any remaining last word 
            await UpdateWithCompleteWordAsync();
            return GetAnalyzeResult();
        }

        public async Task<AnalyzeResult> AnalyzeText(char[] newChars)
        {
            foreach (char c in newChars)
            {
                if (c != '\0') // Ignore null character
                {
                    if (c == WordSplitter)
                    {
                        await UpdateWithCompleteWordAsync();
                    }
                    else
                    {
                        await UpdateWithNewCharacterAsync(c);
                    }
                }
            }
            return GetAnalyzeResult();
        }

        private Task UpdateWithCompleteWordAsync()
        {
            return Task.Run(() =>
                            {
                                lock (_analyserLock)
                                {
                                    if (string.IsNullOrEmpty(_lastWord))
                                        return;

                                    // Increment word count
                                    _numberOfWords++;

                                    // Add word existence frequency
                                    int completeWordExistence;
                                    if (_wordFrequency.ContainsKey(_lastWord))
                                    {
                                        completeWordExistence = _wordFrequency[_lastWord] + 1;
                                        _wordFrequency[_lastWord] = completeWordExistence;
                                    }
                                    else
                                    {
                                        completeWordExistence = 1;
                                        _wordFrequency.Add(_lastWord, 1);
                                    }

                                    // Update popular words list
                                    if (!_popularWordFrequency.ContainsKey(_lastWord))
                                    {
                                        if (_popularWordFrequency.Count < PopularWordListCount)
                                        {
                                            // the popular word list is not long enough, current word was never seeen before, 
                                            // add to popular word list directly
                                            _popularWordFrequency.Add(_lastWord, completeWordExistence);
                                        }
                                        else
                                        {
                                            // check if the completeWord can kick out any popular word if it is not in the popular list
                                            KeyValuePair<string, int> smallestWordCount =
                                                _popularWordFrequency.First(
                                                    kv => kv.Value == _popularWordFrequency.Values.Min());
                                            if (completeWordExistence > smallestWordCount.Value)
                                            {
                                                _popularWordFrequency.Remove(smallestWordCount.Key);
                                                _popularWordFrequency.Add(_lastWord, completeWordExistence);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // already popular, update the count
                                        _popularWordFrequency[_lastWord] = completeWordExistence;
                                    }

                                    // If the complete word is not in the largest word list, and it is larger than the shortest in the list
                                    // replace the shortest word with the completeWord
                                    if (!_largestWords.Contains(_lastWord))
                                    {
                                        if (_largestWords.Count() < LargestWordListCount)
                                        {
                                            _largestWords.Add(_lastWord);
                                        }
                                        else
                                        {
                                            string shortestLargestWord =
                                                _largestWords.FirstOrDefault(s => s.Length < _lastWord.Length);
                                            if (!string.IsNullOrEmpty(shortestLargestWord))
                                            {
                                                _largestWords.Remove(shortestLargestWord);
                                                _largestWords.Add(_lastWord);
                                            }
                                        }
                                    }

                                    // If the complete word is not in the shortest word list, and it is shorter than the largest in the list
                                    // replace the largest word with the completeWord
                                    if (!_smallestWords.Contains(_lastWord))
                                    {
                                        if (_smallestWords.Count() < SmallestWordListCount)
                                        {
                                            _smallestWords.Add(_lastWord);
                                        }
                                        else
                                        {
                                            string largesteShortestWord =
                                                _smallestWords.FirstOrDefault(s => s.Length > _lastWord.Length);
                                            if (!string.IsNullOrEmpty(largesteShortestWord))
                                            {
                                                _smallestWords.Remove(largesteShortestWord);
                                                _smallestWords.Add(_lastWord);
                                            }
                                        }
                                    }

                                    // last word has already been handled, empty the _lastWord variable
                                    _lastWord = string.Empty;
                                }
                            }
                );
        }

        private Task UpdateWithNewCharacterAsync(char newChar)
        {
            return Task.Run(() =>
                            {
                                lock (_analyserLock)
                                {
                                    // increment number of characters
                                    _numberOfCharacters++;

                                    // Add character existence frequency
                                    if (_charFrequency.ContainsKey(newChar))
                                        _charFrequency[newChar] = _charFrequency[newChar] + 1;
                                    else
                                        _charFrequency.Add(newChar, 1);

                                    // add character to last word
                                    var sb = new StringBuilder(_lastWord);
                                    sb.Append(newChar);
                                    _lastWord = sb.ToString();
                                }
                            });
        }

        private AnalyzeResult GetAnalyzeResult()
        {
            List<string> mostFrequentWords = _popularWordFrequency
                .OrderByDescending(w => w.Value)
                .Select(w => w.Key)
                .Take(PopularWordListCount).ToList();
            IEnumerable<string> charFrequency = _charFrequency.OrderByDescending(kvp => kvp.Value)
                .Select(kvp => string.Format("{0} ({1:##,###})", kvp.Key, kvp.Value)).ToList();

            return AnalyzeResult.CeateAnalyzeResult(_numberOfCharacters,
                _numberOfWords, _largestWords,
                _smallestWords, mostFrequentWords,
                charFrequency);
        }
    }
}
