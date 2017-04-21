using System;
using System.Windows.Forms;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.IOServices.Implementations;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.AbstractionLayer.UIServices.Implementations;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
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
