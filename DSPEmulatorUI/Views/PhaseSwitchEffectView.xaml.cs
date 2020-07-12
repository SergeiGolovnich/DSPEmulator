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
    /// Логика взаимодействия для PhaseSwitchEffectView.xaml
    /// </summary>
    public partial class PhaseSwitchEffectView : UserControl
    {
        public PhaseSwitchEffectView()
        {
            InitializeComponent();
        }

        private void RightPhaseSwitched_Checked(object sender, RoutedEventArgs e)
        {
            RightPhaseSwitched.Content = "180°";
        }

        private void RightPhaseSwitched_Unchecked(object sender, RoutedEventArgs e)
        {
            RightPhaseSwitched.Content = "0°";
        }

        private void LeftPhaseSwitched_Checked(object sender, RoutedEventArgs e)
        {
            LeftPhaseSwitched.Content = "180°";
        }

        private void LeftPhaseSwitched_Unchecked(object sender, RoutedEventArgs e)
        {
            LeftPhaseSwitched.Content = "0°";
        }

    }
}
