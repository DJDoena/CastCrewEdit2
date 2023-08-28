using System.Windows;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.FindMergedCastCrew.Main;
using DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations;
using DoenaSoft.DVDProfiler.FindMergedCastCrew.Output;
using DoenaSoft.DVDProfiler.FindMergedCastCrew.Output.Implementations;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Implementations
{
    internal sealed class WindowFactory : IWindowFactory
    {
        public void OpenMainWindow()
        {
            IIOServices ioServices = new IOServices();

            IUIServices uiServices = new WindowUIServices();

            IWebServices webServices = new WebServices();

            IMainModel model = new MainModel(ioServices, uiServices, webServices);

            ILog log = new Log(ioServices);

            IMainViewModel viewModel = new MainViewModel(model, ioServices, uiServices, log, this);

            Window window = new MainWindow();

            window.DataContext = viewModel;

            window.Show();
        }

        public void OpenOutputWindow(ILog log)
        {
            IOutputViewModel viewModel = new OutputViewModel(log);

            Window window = new OutputWindow();

            window.DataContext = viewModel;

            window.Show();
        }
    }
}