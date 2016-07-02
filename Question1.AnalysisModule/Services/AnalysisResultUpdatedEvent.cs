using Microsoft.Practices.Prism.Events;
using Question1.AnalysisModule.Models;

namespace Question1.AnalysisModule.Services
{
    /// <summary>
    /// AnalysisResultUpdatedEvent is used to notify subscribers the AnalysisResult was updated by AnalysisService
    /// Event will be handled by EventAggregator
    /// </summary>
    public class AnalysisResultUpdatedEvent : CompositePresentationEvent<AnalyzeResult>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly AnalysisResultUpdatedEvent _event;

        static AnalysisResultUpdatedEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<AnalysisResultUpdatedEvent>();
        }

        public static AnalysisResultUpdatedEvent Instance
        {
            get { return _event; }
        }
    }
}
