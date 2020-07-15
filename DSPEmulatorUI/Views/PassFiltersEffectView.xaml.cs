using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSPEmulatorUI.Views
{
    /// <summary>
    /// Логика взаимодействия для PassFiltersEffectView.xaml
    /// </summary>
    public partial class PassFiltersEffectView : UserControl
    {
        public PassFiltersEffectView()
        {
            InitializeComponent();
        }

        private void IsHpEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            if (HpFreq == null)
                return;

            HpFreq.IsEnabled = false;
        }

        private void IsHpEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (HpFreq == null)
                return;

            HpFreq.IsEnabled = true;
        }

        private void IsLpEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (LpFreq == null)
                return;

            LpFreq.IsEnabled = true;
        }

        private void IsLpEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LpFreq == null)
                return;

            LpFreq.IsEnabled = false;
        }
    }
}
