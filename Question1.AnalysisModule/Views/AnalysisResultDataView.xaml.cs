using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Question1.AnalysisModule.ViewModels;

namespace Question1.AnalysisModule.Views
{
    /// <summary>
    /// Interaction logic for AnalysisResultDataView.xaml
    /// </summary>
    public partial class AnalysisResultDataView : UserControl, IView
    {
        public AnalysisResultDataView(IAnalyzeResultViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public IViewModel ViewModel
        {
            get { return (IViewModel)DataContext; }
            set { DataContext = value; }
        }
    }
}
