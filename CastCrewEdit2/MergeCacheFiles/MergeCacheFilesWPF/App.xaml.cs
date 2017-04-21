using System.Windows;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.IOServices.Implementations;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.AbstractionLayer.UIServices.Implementations;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IViewModelWPF m_ViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow;
            IUIServices uiServices;
            IIOServices ioServices;
            IModel model;

            base.OnStartup(e);
            uiServices = new WindowUIServices();
            ioServices = new IOServices();
            model = new Model(uiServices, ioServices);
            m_ViewModel = new ViewModelWPF(uiServices, ioServices, model);
            mainWindow = new MainWindow();
            mainWindow.DataContext = m_ViewModel;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            m_ViewModel.Save();
            base.OnExit(e);
        }
    }
}