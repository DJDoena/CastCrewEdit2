using System;
using System.Windows.Forms;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            IUIServices uiServices;
            IIOServices ioServices;
            IModel model;
            IViewModelForms viewModel;

            uiServices = new FormUIServices();
            ioServices = new IOServices();
            model = new Model(uiServices, ioServices);
            viewModel = new ViewModelForms(uiServices, ioServices, model);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(viewModel));
            viewModel.Save();
        }
    }
}
