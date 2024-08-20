using CommunityToolkit.Mvvm.ComponentModel;
using Sounds_UWP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Sounds_UWP.ViewModel
{
    public class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<SoundModel> Sounds { get; private set; }

        private StorageFolder _localFolder;

        public MainPageViewModel()
        {
            Sounds = new ObservableCollection<SoundModel>();
            _localFolder = ApplicationData.Current.LocalFolder;
            _ = InitializeSounds();
        }

        public async Task InitializeSounds()
        {
            List<string> images = new List<string>()
            {
                "expo-go-app.png",
                "443_10_1696353179.jpg",
                "123E231.png",
                "5539177-middle.png",
                "8400_1_1703597400.jpeg",
                "IBM_Simon_Personal_Communicator.png",
                "IMG_0575.jpg"
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
    }
}
