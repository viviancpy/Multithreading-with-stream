using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Question1.AnalysisModule.Models;

namespace Question1.AnalysisModule.Services
{
    /// <summary>
    /// DataAnalysisService provides analysis service to the subscribers through EventAggregator
    /// </summary>
    class DataAnalysisService : IDataAnalysisService, IDisposable
    {
        private readonly ITextAnalyser _analyser;
        private readonly Stream _inputStream;
        private AnalyzeResult _dataAnalyzeResult;

        public AnalyzeResult DataAnalysisResult
        {
            get { return _dataAnalyzeResult; }
        }

        public bool CanStartService { get; private set; }

        public DataAnalysisService(ITextAnalyser analyser, Stream inputStream)
        {
            if (analyser == null)
                throw new ArgumentNullException("analyser");

            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            _analyser = analyser;
            _inputStream = inputStream;
            _dataAnalyzeResult = AnalyzeResult.CreateNewAnalyzeResult();
            CanStartService = true;
        }

        public async Task StartAnalysis()
        {
            CanStartService = false;

            // Use BufferBlock to allow data flow from Stream Reader to TextAnalyser
            var queue = new BufferBlock<char[]>(new DataflowBlockOptions { BoundedCapacity = 10 });

            // _inputStream produces characters (read from stream)
            var producer = Task.Run(() => ProduceFromStream(queue));

            // _analyser consumes the characters produced by _inputStream
            var consumer = Task.Run(() => ConsumeFromStream(queue));

            // Wait for everything to complete.
            await Task.WhenAll(producer, consumer, queue.Completion);
        }

        private async Task ProduceFromStream(BufferBlock<char[]> queue)
        {
            int numRead;
            var buffer = new byte[100000];
            
            while ((numRead = await ReadNewCharacterAsync(buffer, 0, buffer.Length)) != 0)
            {
                char[] charsRead = Encoding.UTF8.GetString(buffer).ToCharArray(0, numRead);
                await queue.SendAsync(charsRead);
            }
            queue.Complete();
        }

        private async Task ConsumeFromStream(BufferBlock<char[]> queue)
        {
            // For every chunk of characters received from the Stream, let TextAnalyzer to analyse it and create a 
            // corresponding AnalysisResult. Publish the result to AnalysisResultUpdatedEvent
            while (await queue.OutputAvailableAsync())
            {
                char[] chars = await queue.ReceiveAsync();
                _dataAnalyzeResult = await _analyser.AnalyzeText(chars);
                if (_dataAnalyzeResult!=null)
                    AnalysisResultUpdatedEvent.Instance.Publish(_dataAnalyzeResult);
            }

            // last word in the analyzer will need to be published to the subscribers
            _dataAnalyzeResult = await _analyser.AnalyzeText();
            if (_dataAnalyzeResult != null)
                AnalysisResultUpdatedEvent.Instance.Publish(_dataAnalyzeResult);
            CanStartService = true;
        }

        private Task<int> ReadNewCharacterAsync(byte[] buffer, int offset, int count)
        {
            return _inputStream.ReadAsync(buffer, offset, count);
        }

        public void Dispose()
        {
            _inputStream.Dispose();
        }


    }
}
