using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Question1.AnalysisModule.Models;
using Question1.AnalysisModule.Services;
using Question1.AnalysisModule.Views;

namespace Question1.AnalysisModule.ViewModels
{
    public class AnalyzeResultViewModel : INotifyPropertyChanged, IAnalyzeResultViewModel, IDisposable
    {
        private int _numberOfCharacters;
        private int _numberOfWords;
        private readonly IDataAnalysisService _service;
        
        public ICommand StartCommand { get; private set; }

        public int NumberOfCharacters
        {
            get { return _numberOfCharacters; }
            set
            {
                if (_numberOfCharacters == value)
                    return;

                _numberOfCharacters = value;
                OnPropertyChanged("NumberOfCharacters");
            }
        }
        public int NumberOfWords
        {
            get { return _numberOfWords; }
            set
            {
                if (_numberOfWords == value)
                    return;

                _numberOfWords = value;
                OnPropertyChanged("NumberOfWords");
            }
        }
        public ObservableCollection<string> LargestWords { get; private set; }
        public ObservableCollection<string> SmallestWords { get; private set; }
        public ObservableCollection<string> PopularWords { get; private set; }
        public ObservableCollection<string> CharacterFrequency { get; private set; }
        public AnalyzeResultViewModel(IDataAnalysisService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            _service = service;
        
            StartCommand = new RelayCommand(
                async _ => await _service.StartAnalysis(),
                _ => _service.CanStartService);

            LargestWords = new ObservableCollection<string>();
            SmallestWords = new ObservableCollection<string>();
            PopularWords = new ObservableCollection<string>();
            CharacterFrequency = new ObservableCollection<string>();
            AnalysisResultUpdatedEvent.Instance.Subscribe(UpdateAnalyzeResult, ThreadOption.UIThread);
        }

        public void UpdateAnalyzeResult(AnalyzeResult newAnalyzeResult)
        {
            if (newAnalyzeResult == null)
                return;
            NumberOfCharacters = newAnalyzeResult.NumberOfCharacters;
            NumberOfWords = newAnalyzeResult.NumberOfWords;

            UpdateCollection(LargestWords, newAnalyzeResult.LargestWords);
            UpdateCollection(SmallestWords, newAnalyzeResult.SmallestWords);
            UpdateCollection(PopularWords, newAnalyzeResult.PopularWords);
            UpdateCollection(CharacterFrequency, newAnalyzeResult.CharacterFrequency);
        }

        private static void UpdateCollection(ObservableCollection<string> target, IEnumerable<string> source)
        {
            if (target == null || source == null)
                return;
            target.Clear();
            foreach (var sourceItem in source.ToArray())
            {
                target.Add(sourceItem);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                Debug.Fail(msg);
            }
        }

        protected virtual bool ThrowOnInvalidPropertyName { get; set; }

        public void Dispose()
        {
            AnalysisResultUpdatedEvent.Instance.Unsubscribe(UpdateAnalyzeResult);
        }
    }
}
