using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLibrary.Data;
using DataLibrary.Model;
using Sounds_UWP.Services;
using Sounds_UWP.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace Sounds_UWP.ViewModel
{
    public class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<SoundModel> Sounds { get; private set; }
        public SoundModel SelectedSound
        {
            get => _selectedSound;
            set
            {
                SetProperty(ref _selectedSound, value);
                if (value != null)
                {
                    NavigateToSoundCommand.Execute(value);
                }
            }
        }
        public ICommand NavigateToSoundCommand { get; private set; }

        private StorageFolder _localFolder;
        private SoundModel _selectedSound;

        public MainPageViewModel()
        {
            Sounds = new ObservableCollection<SoundModel>();
            NavigateToSoundCommand = new RelayCommand<SoundModel>(NavigateToSound);
            _localFolder = ApplicationData.Current.LocalFolder;
            _ = InitializeSounds();
            //LoadSounds();
        }

        private async Task InitializeSounds()
        {
            DatabaseContext context = App.DatabaseContext;
            var sounds = context.MainSounds.Where(s => s.Category == "background").ToList();

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
                    IsEnabled = true
                };

                Sounds.Add(newSound);
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

        private void NavigateToSound(SoundModel sound)
        {
            NavigationService navigationService = new NavigationService();
            navigationService.Navigate(typeof(SoundDetailPage), sound);
        }
    }
}
