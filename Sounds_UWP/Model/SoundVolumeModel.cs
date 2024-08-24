using DataLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace Sounds_UWP.Model
{
    public class SoundVolumeModel : INotifyPropertyChanged
    {
        public SoundVolumeModel(SoundModel soundModel)
        {
            _mediaPlayer = new MediaPlayer();
            _soundModel = soundModel;
        }
        public double Volume
        {
            get => _volume;
            set
            {
                if (Math.Abs(_volume - value) > 0.01)
                {
                    _volume = value;
                    _mediaPlayer.Volume = _volume / 10;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }
        public string BackgroundUri => _soundModel.BackgroundUri;
        public int SoundId => _soundModel.Id;
        public MediaPlayer MediaPlayer {  get { return _mediaPlayer; } }
        private SoundModel _soundModel;
        private MediaPlayer _mediaPlayer;
        private double _volume;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
