using System.Collections.Generic;

namespace Question1.AnalysisModule.Models
{
    /// <summary>
    /// AnalyzeResult is the type that we use to communicate between the worker and the Interface. 
    /// It contains the information required by the GUI to update
    /// </summary>
    public class AnalyzeResult
    {
        public int NumberOfCharacters { get; set; }
        public int NumberOfWords { get; set; }
        public IEnumerable<string> LargestWords { get; set; }
        public IEnumerable<string> SmallestWords { get; set; }
        public IEnumerable<string> PopularWords { get; set; }
        public IEnumerable<string> CharacterFrequency { get; set; }

        protected AnalyzeResult()
        {
            LargestWords = new List<string>();
            SmallestWords = new List<string>();
            PopularWords = new List<string>();
            CharacterFrequency = new List<string>();
        }

        public static AnalyzeResult CreateNewAnalyzeResult()
        {
            return new AnalyzeResult();
        }

        public static AnalyzeResult CeateAnalyzeResult(
            int numberOfCharacters,
            int numberOfWords,
            IEnumerable<string> largestWords,
            IEnumerable<string> smallestWords,
            IEnumerable<string> popularwords,
            IEnumerable<string> characterFrequency)
        {
            return new AnalyzeResult
            {
                NumberOfCharacters = numberOfCharacters,
                NumberOfWords = numberOfWords,
                LargestWords = largestWords,
                SmallestWords = smallestWords,
                PopularWords = popularwords,
                CharacterFrequency = characterFrequency
            };
        }
    }
}
