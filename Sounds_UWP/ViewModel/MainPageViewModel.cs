using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sounds_UWP.Model;
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
        }

        private async Task InitializeSounds()
        {
            List<string> images = new List<string>()
            {
                "expo-go-app.png",
                "443_10_1696353179.jpg",
                "123E231.png",
                "5539177-middle.png",
                "8400_1_1703597400.jpeg",
                "IBM_Simon_Personal_Communicator.png",
                "IMG_0575.jpg",
                "expo-go-app.png",
                "443_10_1696353179.jpg",
                "123E231.png",
                "5539177-middle.png",
                "8400_1_1703597400.jpeg",
                "IBM_Simon_Personal_Communicator.png",
                "IMG_0575.jpg",
                "5539177-middle.png",
                "vliublennye-svidanie-siluety-paren-i-devushka-bereg-more-zve.jpg"
            };

            foreach (var imageName in images)
            {
                try
                {
                    StorageFile imageFile = await _localFolder.GetFileAsync(imageName);
                }
                catch (FileNotFoundException)
                {
                    StorageFile sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{imageName}"));
                    await sourceFile.CopyAsync(_localFolder);
                }
            }

            for (int i = 0; i < images.Count; i++)
            {
                StorageFile imageFile = await _localFolder.GetFileAsync(images[i]);

                Sounds.Add(new SoundModel { Name = $"Звук {i + 1}", BackgroundUri = imageFile.Path });
            }
        }

        private void NavigateToSound(SoundModel sound)
        {
            NavigationService navigationService = new NavigationService();
            navigationService.Navigate(typeof(SoundDetailPage), sound);
        }
    }
}
