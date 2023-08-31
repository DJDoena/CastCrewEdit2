using System;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal sealed partial class MainForm : Form
    {
        private readonly IViewModelForms m_ViewModel;

        public MainForm(IViewModelForms vieModel)
        {
            m_ViewModel = vieModel;
            m_ViewModel.LeftFileNameChanged += OnLeftFileNameChanged;
            m_ViewModel.RightFileNameChanged += OnRightFileNameChanged;
            m_ViewModel.TargetFileNameChanged += OnTargetFileNameChanged;
            InitializeComponent();
            Icon = Properties.Resource.djdsoft;
        }

        private void OnLeftFileNameChanged(object sender, EventArgs e)
        {
            LeftFileTextBox.Text = m_ViewModel.LeftFileName;
            ValidateButtons();
        }

        private void OnRightFileNameChanged(object sender, EventArgs e)
        {
            RightFileTextBox.Text = m_ViewModel.RightFileName;
            ValidateButtons();
        }

        private void OnTargetFileNameChanged(object sender, EventArgs e)
        {
            TargetFileTextBox.Text = m_ViewModel.TargetFileName;
            ValidateButtons();
        }

        private void ValidateButtons()
        {
            MergeButton.Enabled = m_ViewModel.CanExecuteMerge();
            MergeThirdFileButton.Enabled = m_ViewModel.CanExecuteMergeIntoThirdFile();
            ClearFileNamesButton.Enabled = m_ViewModel.CanExecuteClearFileNames();
        }

        private void OnMainFormLoad(object sender
            , EventArgs e)
        {
            OnLeftFileNameChanged(this, EventArgs.Empty);
            OnRightFileNameChanged(this, EventArgs.Empty);
            OnTargetFileNameChanged(this, EventArgs.Empty);
        }

        private void OnSelectLeftFileButtonClick(object sender
            , EventArgs e)
        {
            m_ViewModel.SelectLeftFileName();
        }

        private void OnSelectRightFileButtonClick(object sender
            , EventArgs e)
        {
            m_ViewModel.SelectRightFileName();
        }

        private void OnSelectTargetFileButtonClick(object sender
            , EventArgs e)
        {
            m_ViewModel.SelectTargetFileName();
        }

        private void OnMergeThirdFileButtonClick(object sender
            , EventArgs e)
        {
            m_ViewModel.MergeThirdFile();
        }

        private void OnMergeButtonClick(object sender
            , EventArgs e)
        {
            m_ViewModel.Merge();
        }

        private void OnClearFileNamesButtonClick(object sender
            , EventArgs e)
        {
            m_ViewModel.ClearFileNames();
        }
    }
}