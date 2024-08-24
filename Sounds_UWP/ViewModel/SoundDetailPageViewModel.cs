using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLibrary.Data;
using DataLibrary.Model;
using Sounds_UWP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;

namespace Sounds_UWP.ViewModel
{
    public class SoundDetailPageViewModel : ObservableObject
    {
        public SoundModel SelectedSound { get; set; }
        public ICommand NavigateToBackCommand { get; private set; }
        public ICommand PlayPauseCommand { get; private set; }
        public ICommand ShowTimerPanelCommand { get; private set; }
        public ICommand SetTimerCommand { get; private set; }
        public ICommand ShowMelodiesPanelCommand { get; private set; }
        public ICommand AddExtraMelodyCommand {  get; private set; }
        public ObservableCollection<SoundModel> AnimalsSounds { get; private set; }
        public ObservableCollection<SoundModel> InstrumentalSounds { get; private set; }
        public ObservableCollection<SoundModel> NatureSounds { get; private set; }
        public ObservableCollection<SoundModel> ExtraSounds { get; private set; }


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
        public bool IsPanelMelodiesVisible
        {
            get => _isPanelMelodiesVisible;
            set => SetProperty(ref _isPanelMelodiesVisible, value);
        }
        public bool IsAddButtonVisible
        {
            get => _isAddButtonVisible;
            set => SetProperty(ref _isAddButtonVisible, value);
        }

        private MediaPlayer _playerBack;
        private List<MediaPlayer> _players;
        private bool _isPlaying;
        private int _extraSongsCounter;

        private DispatcherTimer _timer;
        private TimeSpan _timeSpan;

        private bool _isTimerButtonVisible;
        private bool _isPanelTimerVisible;
        private bool _isTextCountdownVisible;
        private string _textCountdown;

        private bool _isPanelMelodiesVisible;
        private bool _isAddButtonVisible;

        private StorageFolder _localFolder;

        public SoundDetailPageViewModel(SoundModel selectedSound)
        {
            SelectedSound = selectedSound;
            _extraSongsCounter = 0;

            NavigateToBackCommand = new RelayCommand(NavigateToBack);
            PlayPauseCommand = new RelayCommand(PlayPause);
            ShowTimerPanelCommand = new RelayCommand(ShowTimerPanel);
            SetTimerCommand = new RelayCommand<object>(SetTimer);
            ShowMelodiesPanelCommand = new RelayCommand(ShowMelodiesPanel);
            AddExtraMelodyCommand = new RelayCommand<SoundModel>(AddExtraMelody);

            AnimalsSounds = new ObservableCollection<SoundModel>();
            InstrumentalSounds = new ObservableCollection<SoundModel>();
            NatureSounds = new ObservableCollection<SoundModel>();
            _players = new List<MediaPlayer>();
            ExtraSounds = new ObservableCollection<SoundModel>();

            IsTimerButtonVisible = true;
            IsPanelTimerVisible = false;
            IsTextCountdownVisible = false;

            IsPanelMelodiesVisible = false;
            IsAddButtonVisible = true;

            _localFolder = ApplicationData.Current.LocalFolder;

            InitializeMediaPlayer(_playerBack, SelectedSound.SoundUri);
            _ = InitializeMelodies();
        }

        private void NavigateToBack()
        {
            NavigationService navigationService = new NavigationService();
            navigationService.NavigateToBack();
            PlayerStop();
        }
        private void PlayPause()
        {
            foreach (var player in _players)
            {
                if (_isPlaying)
                {
                    player.Pause();
                }
                else
                {
                    player.Play();
                }
            }
            
            IsPlaying = !IsPlaying;
        }

        private void InitializeMediaPlayer(MediaPlayer player, string uri)
        {
            player = new MediaPlayer();
            player.Source = MediaSource.CreateFromUri(new Uri(uri));
            player.IsLoopingEnabled = true;
            IsPlaying = true;
            player.Play();
            _players.Add(player);
        }

        private void AddExtraMelody(SoundModel sound)
        {
            _extraSongsCounter++;
            if (_extraSongsCounter <= 3)
            {
                _players.Add(new MediaPlayer());
                InitializeMediaPlayer(_players[_players.Count - 1], sound.SoundUri);
                IsPanelMelodiesVisible = !IsPanelMelodiesVisible;
                sound.IsEnabled = false;
                ExtraSounds.Add(sound);
            }
        }

        private async Task InitializeMelodies()
        {
            DatabaseContext context = App.DatabaseContext;
            var sounds = context.MainSounds.Where(s => s.Category == "Animals" || s.Category == "Instrumental" || s.Category == "Nature").ToList();

            foreach (var sound in sounds)
            {
                StorageFile imageFile = await GetOrCopyFile(sound.BackgroundUri, "img");
                StorageFile soundFile = await GetOrCopyFile(sound.SoundUri, "sounds");

                SoundModel newSound = new SoundModel
                {
                    Name = sound.Name,
                    BackgroundUri = imageFile.Path,
                    SoundUri = soundFile.Path,
                    Category = sound.Category,
                    IsEnabled = true
                };

                if (sound.Category == "Animals")
                    AnimalsSounds.Add(newSound);

                else if (sound.Category == "Instrumental")
                    InstrumentalSounds.Add(newSound);

                else if (sound.Category == "Nature")
                    NatureSounds.Add(newSound);
            }
        }

        private async Task<StorageFile> GetOrCopyFile(string uri, string folder)
        {
            try
            {
                return await _localFolder.GetFileAsync(uri);
            }
            catch (FileNotFoundException)
            {
                StorageFile sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{folder}/{uri}"));
                await sourceFile.CopyAsync(_localFolder);
                return sourceFile;
            }
        }

        private void PlayerStop()
        {
            foreach (var player  in _players)
            {
                player.Pause();
                player.PlaybackSession.Position = TimeSpan.Zero;
            }
            
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
        private void ShowMelodiesPanel()
        {
            if (_extraSongsCounter == 2)
            {
                IsAddButtonVisible = !IsAddButtonVisible;
            }
            IsPanelMelodiesVisible = !IsPanelMelodiesVisible;
        }
    }
}
