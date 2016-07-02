using System.Threading.Tasks;
using Question1.AnalysisModule.Models;

namespace Question1.AnalysisModule.Services
{
    public interface IDataAnalysisService
    {
        AnalyzeResult DataAnalysisResult { get; }
        bool CanStartService { get; }
        Task StartAnalysis();
    }
}
