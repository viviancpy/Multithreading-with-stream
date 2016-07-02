using System.IO;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Question1.AnalysisModule.Services;
using Question1.AnalysisModule.ViewModels;
using Question1.AnalysisModule.Views;

namespace Question1.AnalysisModule
{
    public class DataAnalysisModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public DataAnalysisModule(IUnityContainer container, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _container = container;
        }

        public void Initialize()
        {
            
            _container.RegisterType<ITextAnalyser, TextAnalyser>();
            _container.RegisterType<Stream, DevTest.LorumIpsumStream>(new InjectionConstructor());
            _container.RegisterType<IDataAnalysisService, DataAnalysisService>();
            _container.RegisterType<AnalysisResultDataView>();

            _container.RegisterType<IAnalyzeResultViewModel, AnalyzeResultViewModel>();

            _regionManager.RegisterViewWithRegion("AnalysisResultRegion", typeof(AnalysisResultDataView));

        }
    }
}
