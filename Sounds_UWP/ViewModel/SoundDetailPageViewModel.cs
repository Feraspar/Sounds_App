using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sounds_UWP.Model;
using Sounds_UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sounds_UWP.ViewModel
{
    public class SoundDetailPageViewModel : ObservableObject
    {
        public SoundModel SelectedSound { get; set; }
        public ICommand NavigateToBackCommand { get; private set; }

        public SoundDetailPageViewModel(SoundModel selectedSound)
        {
            SelectedSound = selectedSound;
            NavigateToBackCommand = new RelayCommand(NavigateToBack);
        }

        private void NavigateToBack()
        {
            NavigationService navigationService = new NavigationService();
            navigationService.NavigateToBack();
        }
    }
}
