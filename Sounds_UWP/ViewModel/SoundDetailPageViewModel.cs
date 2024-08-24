using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLibrary.Data;
using DataLibrary.Model;
using Sounds_UWP.Model;
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
    /// <summary>
    /// ViewModel для страницы с фоновой музыкой
    /// </summary>
    public class SoundDetailPageViewModel : ObservableObject
    {
        public SoundModel SelectedSound { get; set; }
        public ICommand NavigateToBackCommand { get; private set; }
        public ICommand PlayPauseCommand { get; private set; }
        public ICommand ShowTimerPanelCommand { get; private set; }
        public ICommand SetTimerCommand { get; private set; }
        public ICommand ShowMelodiesPanelCommand { get; private set; }
        public ICommand AddExtraMelodyCommand {  get; private set; }
        public ICommand ShowVolumeCommand {  get; private set; }
        public ICommand DeleteSoundCommand { get; private set; }
        public ObservableCollection<SoundModel> AnimalsSounds { get; private set; }
        public ObservableCollection<SoundModel> InstrumentalSounds { get; private set; }
        public ObservableCollection<SoundModel> NatureSounds { get; private set; }
        public ObservableCollection<SoundModel> ExtraSounds { get; private set; }
        public ObservableCollection<SoundVolumeModel> BackAndExtraSounds { get; private set; }


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
        public bool IsPanelVolumeVisible
        {
            get => _isPanelVolumeVisible;
            set => SetProperty(ref _isPanelVolumeVisible, value);
        }

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
        private bool _isPanelVolumeVisible;

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
            ShowVolumeCommand = new RelayCommand(ShowVolume);
            DeleteSoundCommand = new RelayCommand<SoundVolumeModel>(DeleteSound);

            AnimalsSounds = new ObservableCollection<SoundModel>();
            InstrumentalSounds = new ObservableCollection<SoundModel>();
            NatureSounds = new ObservableCollection<SoundModel>();
            ExtraSounds = new ObservableCollection<SoundModel>();

            SoundVolumeModel soundVolumeModel = new SoundVolumeModel(SelectedSound);
            BackAndExtraSounds = new ObservableCollection<SoundVolumeModel>()
            {
                soundVolumeModel
            };

            IsTimerButtonVisible = true;
            IsPanelTimerVisible = false;
            IsTextCountdownVisible = false;

            IsPanelMelodiesVisible = false;
            IsAddButtonVisible = true;

            _localFolder = ApplicationData.Current.LocalFolder;

            InitializeMediaPlayer(soundVolumeModel, SelectedSound.SoundUri);
            _ = InitializeMelodies();
        }

        private void NavigateToBack()   // Навигация
        {
            NavigationService navigationService = new NavigationService();
            navigationService.NavigateToBack();
            PlayerStop();
        }
        private void PlayPause()    // Логика старта/паузы
        {
            foreach (var soundVolumeModel in BackAndExtraSounds)
            {
                if (_isPlaying)
                {
                    soundVolumeModel.MediaPlayer.Pause();
                }
                else
                {
                    soundVolumeModel.MediaPlayer.Play();
                }
            }
            
            IsPlaying = !IsPlaying;
        }

        private void InitializeMediaPlayer(SoundVolumeModel soundVolumeModel, string uri)   // Установка звука в плеер
        {
            MediaPlayer player = soundVolumeModel.MediaPlayer;
            player.Source = MediaSource.CreateFromUri(new Uri(uri));
            player.IsLoopingEnabled = true;
            IsPlaying = true;
            player.Play();
            soundVolumeModel.Volume = 5;
        }

        private void AddExtraMelody(SoundModel sound)   // Добавление доп. звука
        {
            _extraSongsCounter++;
            if (_extraSongsCounter <= 3)
            {
                var soundVolumeModel = new SoundVolumeModel(sound);
                InitializeMediaPlayer(soundVolumeModel, sound.SoundUri);
                BackAndExtraSounds.Add(soundVolumeModel);
                IsPanelMelodiesVisible = !IsPanelMelodiesVisible;
                sound.IsEnabled = false;
                ExtraSounds.Add(sound);
            }
        }

        private async Task InitializeMelodies() // Загрузка звуков из БД
        {
            DatabaseContext context = App.DatabaseContext;
            var sounds = context.MainSounds.Where(s => s.Category == "Animals" || s.Category == "Instrumental" || s.Category == "Nature").ToList();

            foreach (var sound in sounds)
            {
                StorageFile imageFile = await GetOrCopyFile(sound.BackgroundUri, "img");
                StorageFile soundFile = await GetOrCopyFile(sound.SoundUri, "sounds");

                SoundModel newSound = new SoundModel
                {
                    Id = sound.Id,
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
            foreach (var soundVolumeModel in BackAndExtraSounds)
            {
                soundVolumeModel.MediaPlayer.Pause();
                soundVolumeModel.MediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
            }
            
            IsPlaying = false;
        }

        private void DeleteSoundFromPlayer(SoundVolumeModel soundVolumeModel)   // Удаление звука из плеера
        {
            soundVolumeModel.MediaPlayer.Pause();
            soundVolumeModel.MediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
        }

        private void ShowTimerPanel()
        {
            IsTimerButtonVisible = !IsTimerButtonVisible;
            IsPanelTimerVisible = !IsPanelTimerVisible;
        }

        private void SetTimer(object parameter) // Назнчение таймера
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

        private void ShowVolume()
        {
            IsPanelVolumeVisible = !IsPanelVolumeVisible;
        }

        private void DeleteSound(SoundVolumeModel soundVolumeModel) // Удаление доп.звука из коллекции
        {
            _extraSongsCounter--;
            if (IsAddButtonVisible == false)
                IsAddButtonVisible = !IsAddButtonVisible;

            SoundModel soundToRemove = null;

            foreach (var sound in ExtraSounds)
            {
                if (soundVolumeModel.SoundId == sound.Id)
                {
                    sound.IsEnabled = true;
                    soundToRemove = sound;
                    break;
                }
            }

            ExtraSounds.Remove(soundToRemove);
            BackAndExtraSounds.Remove(soundVolumeModel);
            DeleteSoundFromPlayer(soundVolumeModel);
        }
    }
}
