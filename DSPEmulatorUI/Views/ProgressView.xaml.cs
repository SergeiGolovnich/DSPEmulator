using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSPEmulatorUI.Views
{
    /// <summary>
    /// Логика взаимодействия для ProgressView.xaml
    /// </summary>
    public partial class ProgressView : Window
    {
        private readonly BackgroundWorker BackgroundWorker;
        public ProgressView(BackgroundWorker backgroundWorker)
        {
            InitializeComponent();
            this.Closing += ProgressView_Closing;

            BackgroundWorker = backgroundWorker;
            BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            BackgroundWorker.RunWorkerAsync();
        }

        private void ProgressView_Closing(object sender, CancelEventArgs e)
        {
            BackgroundWorker.CancelAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker.CancelAsync();
        }
    }
}
