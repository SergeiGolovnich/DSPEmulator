using DSPEmulatorUI.ViewModels;
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
    /// Логика взаимодействия для PlayerView.xaml
    /// </summary>
    public partial class PlayerView : UserControl
    {
        bool isUserInput = false;
        BitmapImage playImage, pauseImage;
        public PlayerView()
        {
            InitializeComponent();
           
            playImage = new BitmapImage(new Uri("pack://application:,,,/Views/Icons/play-button.png"));
            pauseImage = new BitmapImage(new Uri("pack://application:,,,/Views/Icons/pause-button.png"));
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PlayerViewModel)?.PlayPauseButton();

            if ((DataContext as PlayerViewModel)?.AudioPlayer.IsPlaying ?? false)
            {
                PlayPauseImage.Source = pauseImage;
            }
            else
            {
                PlayPauseImage.Source = playImage;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PlayerViewModel)?.StopButton();

            PlayPauseImage.Source = playImage;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isUserInput && DataContext != null)
            {
                (DataContext as PlayerViewModel).AudioPlayer.PositionPercent = (sender as Slider).Value;
            }
        }

        private void Slider_GotMouseCapture(object sender, MouseEventArgs e)
        {
            isUserInput = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PlayerViewModel)?.NextButton();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PlayerViewModel)?.PreviousButton();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as PlayerViewModel).AudioPlayer.AudioFileChanged += AudioPlayer_AudioFileChanged;
        }

        private void AudioPlayer_AudioFileChanged(object sender, string e)
        {
            PlayPauseImage.Source = pauseImage;
        }

        private void Slider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            isUserInput = false;
        }
    }
}
