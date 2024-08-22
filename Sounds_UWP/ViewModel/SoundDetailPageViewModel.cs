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

namespace Sounds_UWP.ViewModel
{
    public class SoundDetailPageViewModel : ObservableObject
    {
        public SoundModel SelectedSound { get; set; }
        public ICommand NavigateToBackCommand { get; private set; }
        public ICommand PlayPauseCommand { get; private set; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        private MediaPlayer _player;
        private bool _isPlaying;

        public SoundDetailPageViewModel(SoundModel selectedSound)
        {
            SelectedSound = selectedSound;
            NavigateToBackCommand = new RelayCommand(NavigateToBack);
            PlayPauseCommand = new RelayCommand(PlayPause);
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
    }
}
