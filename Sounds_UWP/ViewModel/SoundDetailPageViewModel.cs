using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLibrary.Model;
using Sounds_UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;

namespace Sounds_UWP.ViewModel
{
    public class SoundDetailPageViewModel : ObservableObject
    {
        public SoundModel SelectedSound { get; set; }
        public ICommand NavigateToBackCommand { get; private set; }
        public ICommand PlayPauseCommand { get; private set; }
        public ICommand ShowTimerPanelCommand { get; private set; }
        public ICommand SetTimerCommand { get; private set; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }
        public bool IsTimerButtonVisible
        {
            get => _isTimerButtonVisible;
            set => SetProperty(ref _isTimerButtonVisible, value);
        }
        public bool IsPanelTimerVisible
        {
            get => _isPanelTimerVisible;
            set => SetProperty(ref _isPanelTimerVisible, value);
        }
        public bool IsTextCountdownVisible
        {
            get => _isTextCountdownVisible;
            set => SetProperty(ref _isTextCountdownVisible, value);
        }
        public string TextCountdown
        {
            get => _textCountdown;
            set => SetProperty(ref _textCountdown, value);
        }

        private MediaPlayer _player;
        private bool _isPlaying;

        private DispatcherTimer _timer;
        private TimeSpan _timeSpan;

        private bool _isTimerButtonVisible;
        private bool _isPanelTimerVisible;
        private bool _isTextCountdownVisible;
        private string _textCountdown;

        public SoundDetailPageViewModel(SoundModel selectedSound)
        {
            SelectedSound = selectedSound;
            NavigateToBackCommand = new RelayCommand(NavigateToBack);
            PlayPauseCommand = new RelayCommand(PlayPause);
            ShowTimerPanelCommand = new RelayCommand(ShowTimerPanel);
            SetTimerCommand = new RelayCommand<object>(SetTimer);

            IsTimerButtonVisible = true;
            IsPanelTimerVisible = false;
            IsTextCountdownVisible = false;

            InitializeMediaPlayer();
        }

        private void NavigateToBack()
        {
            NavigationService navigationService = new NavigationService();
            navigationService.NavigateToBack();
            PlayerStop();
        }
        private void PlayPause()
        {
            if (_isPlaying)
            {
                _player.Pause();
            }
            else
            {
                _player.Play();
            }
            IsPlaying = !IsPlaying;
        }

        private void InitializeMediaPlayer()
        {
            _player = new MediaPlayer();
            _player.Source = MediaSource.CreateFromUri(new Uri(SelectedSound.SoundUri));
            _player.IsLoopingEnabled = true;
            IsPlaying = true;
            _player.Play();
        }

        private void PlayerStop()
        {
            _player.Pause();
            _player.PlaybackSession.Position = TimeSpan.Zero;
            IsPlaying = false;
        }

        private void ShowTimerPanel()
        {
            IsTimerButtonVisible = !IsTimerButtonVisible;
            IsPanelTimerVisible = !IsPanelTimerVisible;
        }

        private void SetTimer(object parameter)
        {
            int minutes = Convert.ToInt32(parameter);
            _timeSpan = TimeSpan.FromMinutes(minutes);
            TextCountdown = _timeSpan.ToString(@"mm\:ss");

            IsPanelTimerVisible = !IsPanelTimerVisible;
            IsTextCountdownVisible = !IsTextCountdownVisible;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            if (_timeSpan.TotalSeconds > 0)
            {
                _timeSpan = _timeSpan.Subtract(TimeSpan.FromSeconds(1));
                TextCountdown = _timeSpan.ToString(@"mm\:ss");
            }
            else
            {
                _timer.Stop();
                PlayerStop();
                IsTextCountdownVisible = !IsTextCountdownVisible;
                IsTimerButtonVisible = !IsTimerButtonVisible;
            }
        }
    }
}
