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
            //using (DatabaseContext context = App.DatabaseContext)
            //{
            //    var sounds = context.MainSounds.ToList();

            //    foreach (var soundName in sounds)
            //    {
            //        try
            //        {
            //            StorageFile imageFile = await _localFolder.GetFileAsync(soundName.BackgroundUri);
            //            StorageFile soundFile = await _localFolder.GetFileAsync(soundName.SoundUri);
            //        }
            //        catch (FileNotFoundException)
            //        {
            //            StorageFile imageSourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/img/{soundName.BackgroundUri}"));
            //            await imageSourceFile.CopyAsync(_localFolder);

            //            StorageFile soundSourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/sounds/{soundName.SoundUri}"));
            //            await soundSourceFile.CopyAsync(_localFolder);
            //        }
            //    }

            //    foreach (var sound in sounds)
            //    {
            //        StorageFile imageFile = await _localFolder.GetFileAsync(sound.BackgroundUri);
            //        StorageFile soundFile = await _localFolder.GetFileAsync(sound.SoundUri);

            //        var newSound = new SoundModel
            //        {
            //            Name = sound.Name,
            //            BackgroundUri = imageFile.Path,
            //            SoundUri = soundFile.Path
            //        };

            //        Sounds.Add(newSound);
            //    }
            //}

            DatabaseContext context = App.DatabaseContext;
            var sounds = context.MainSounds.ToList();

            foreach (var soundName in sounds)
            {
                try
                {
                    StorageFile imageFile = await _localFolder.GetFileAsync(soundName.BackgroundUri);
                    StorageFile soundFile = await _localFolder.GetFileAsync(soundName.SoundUri);
                }
                catch (FileNotFoundException)
                {
                    StorageFile imageSourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/img/{soundName.BackgroundUri}"));
                    await imageSourceFile.CopyAsync(_localFolder);

                    StorageFile soundSourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/sounds/{soundName.SoundUri}"));
                    await soundSourceFile.CopyAsync(_localFolder);
                }
            }

            foreach (var sound in sounds)
            {
                StorageFile imageFile = await _localFolder.GetFileAsync(sound.BackgroundUri);
                StorageFile soundFile = await _localFolder.GetFileAsync(sound.SoundUri);

                var newSound = new SoundModel
                {
                    Name = sound.Name,
                    BackgroundUri = imageFile.Path,
                    SoundUri = soundFile.Path
                };

                Sounds.Add(newSound);
            }
        }

        private void NavigateToSound(SoundModel sound)
        {
            NavigationService navigationService = new NavigationService();
            navigationService.Navigate(typeof(SoundDetailPage), sound);
        }
    }
}
