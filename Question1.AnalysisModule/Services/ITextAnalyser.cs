using System.Threading.Tasks;
using Question1.AnalysisModule.Models;

namespace Question1.AnalysisModule.Services
{
    public interface ITextAnalyser
    {
        Task<AnalyzeResult> AnalyzeText(char[] newChars);
        Task<AnalyzeResult> AnalyzeText();
    }
}
