using Question1.AnalysisModule.ViewModels;

namespace Question1.AnalysisModule.Views
{
    public interface IView
    {
        IViewModel ViewModel { get; set; }
    }
}
