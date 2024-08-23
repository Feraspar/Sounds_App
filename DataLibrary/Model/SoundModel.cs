using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Model
{
    public class SoundModel : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string BackgroundUri
        {
            get => _backgroundUri;
            set
            {
                if (_backgroundUri != value)
                {
                    _backgroundUri = value;
                    OnPropertyChanged(nameof(BackgroundUri));
                }
            }
        }
        public string SoundUri
        {
            get => _soundUri;
            set
            {
                if (_soundUri != value)
                {
                    _soundUri = value;
                    OnPropertyChanged(nameof(SoundUri));
                }
            }
        }
        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        private string _name;
        private string _backgroundUri;
        private string _soundUri;
        private string _category;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
