using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        private List<string> musicList;
        private int currentTrackIndex;
        private bool isPlaying;
        private bool isRandom;
        private bool isRepeat;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            musicList = new List<string>();
            currentTrackIndex = 0;
            isPlaying = false;
            isRandom = false;
            isRepeat = false;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private List<string> history = new List<string>();

        private void PlayMusic()
        {
            string trackPath = musicList[currentTrackIndex];
            mediaElement.Source = new Uri(trackPath);
            mediaElement.Play();
            isPlaying = true;
            PlayPauseButton.Content = "Пауза";
            timer.Start();
        }

        private void PausePlay()
        {
            mediaElement.Pause();
            isPlaying = false;
            PlayPauseButton.Content = "Продолжить";
            timer.Stop();
        }

        private void ContinuePlay()
        {
            mediaElement.Play();
            isPlaying = true;
            PlayPauseButton.Content = "Пауза";
            timer.Start();
        }
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav";

            if (openFileDialog.ShowDialog() == true)
            {
                string[] files = openFileDialog.FileNames;

                musicList.Clear();
                musicList.AddRange(files);

                currentTrackIndex = 0;
                history.Add(files[currentTrackIndex]);
                PlayMusic();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (musicList.Count == 0)
            {
                MessageBox.Show("История прослушивания пуста");
                return;
            }

            HistoryListBox.Items.Clear();
            foreach (string track in musicList)
            {
                HistoryListBox.Items.Add(System.IO.Path.GetFileName(track));
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                PausePlay();
            }
            else
            {
                ContinuePlay();
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                PlayMusic();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex < musicList.Count - 1)
            {
                currentTrackIndex++;
                PlayMusic();
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string trackPath = musicList[currentTrackIndex];
                mediaElement.Source = new Uri(trackPath);
                mediaElement.Play();
                isPlaying = true;
                timer.Start();

                isRepeat = !isRepeat;
                RepeatButton.Content = isRepeat ? "Повтор" : "Повторить еще раз?";
            }
            catch (System.ArgumentOutOfRangeException)
            { }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            isRandom = !isRandom;
            RandomButton.Content = isRandom ? "Случайно" : "Еще раз перемешать?";

            if (isRandom)
            {
                var rng = new Random();
                musicList = musicList.OrderBy(x => rng.Next()).ToList();
            }
            else
            {
                musicList.Sort();
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds(PositionSlider.Value);
                mediaElement.Position = newPosition;
            }
        }


        private void VolumeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = VolumeSlider.Value;
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan currentPosition = mediaElement.Position;
                TimeSpan totalDuration = mediaElement.NaturalDuration.TimeSpan;

                PositionSlider.Minimum = 0;
                PositionSlider.Maximum = totalDuration.TotalSeconds;
                PositionSlider.Value = currentPosition.TotalSeconds;

                RemainingTimeLabel.Content = "-" + (totalDuration - currentPosition).ToString(@"mm\:ss");
                CurrentTimeLabel.Content = currentPosition.ToString(@"mm\:ss");
            }
        }

    }
}