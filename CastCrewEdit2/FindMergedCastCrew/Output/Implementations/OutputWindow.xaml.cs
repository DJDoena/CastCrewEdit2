using System;
using System.Windows;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Output
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        public OutputWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(Object sender
            , RoutedEventArgs e)
        {
            var viewModel = (IOutputViewModel)DataContext;

            WB.NavigateToString(viewModel.Source);
        }
    }
}
