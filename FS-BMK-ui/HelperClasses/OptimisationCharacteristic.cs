using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS_BMK_ui.HelperClasses
{
    public class OptimisationCharacteristic : INotifyPropertyChanged
    {
        //    private string _name;
        //    public string Name { get; set; }

        private string _sumName = "SignificanceSum";

        private float _weightFactor;
        public float WeightFactor
        {
            get { return _weightFactor; }
            set { _weightFactor = value; }
        }

        private float _significance;
        public float Significance
        {
            get { return _significance; }
            set { _significance = value; OnPropertyChanged(_sumName); }
        }

        private float _peakFlatness;
        public float PeakFlatness
        {
            get { return _peakFlatness; }
            set { _peakFlatness = value; }
        }

        private float _peakWidth;
        public float PeakWidth
        {
            get { return _peakWidth; }
            set { _peakWidth = value; }
        }

        private float _target;
        public float Target
        {
            get { return _target; }
            set { _target = value; }
        }



        public OptimisationCharacteristic(float significance)
        {
            Significance = significance;
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
