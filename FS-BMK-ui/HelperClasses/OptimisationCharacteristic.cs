using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FS_BMK_ui.HelperClasses
{
    public class OptimisationCharacteristic : INotifyPropertyChanged
    {
        //    private string _name;
        //    

        private string _name;
        public string Name { get { return _name; } set { _name = value; } }


        public OptimisationCharacteristic(string name, float target, float peakWidth, float peakFlatness, float lower, float upper)
        {
            _name = name;
            _peakWidth = peakWidth;
            _peakFlatness = peakFlatness;
            _target = target;
            _lower = lower;
            _upper = upper;
        }

        private float _lower;
        public float Lower
        {
            get { return _lower; }
            set { _lower = value; OnPropertyChanged("Lower"); }
        }

        private float _upper;
        public float Upper
        {
            get { return _upper; }
            set { _upper = value; OnPropertyChanged("Upper"); }
        }


        private float _peakFlatness;
        public float PeakFlatness
        {
            get { return _peakFlatness; }
            set { _peakFlatness = value; OnPropertyChanged("PeakFlatness"); }
            
        }

        private float _peakWidth;
        public float PeakWidth
        {
            get { return _peakWidth; }
            set { _peakWidth = value; OnPropertyChanged("PeakWidth"); }
        }

        private float _target;
        public float Target
        {
            get { return _target; }
            set { _target = value; OnPropertyChanged("Target"); }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
