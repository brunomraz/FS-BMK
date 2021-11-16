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

        private static string _sumName = "SignificanceSum";
        private static float _sum;
        public static float Sum
        {
            get { return _sum; }
            set { _sum = value; }
        }

        private float _weightFactor;
        public float WeightFactor
        {
            get { return _weightFactor; }
            //set { _weightFactor = value; }
        }

        private float _significance;
        public float Significance
        {
            get
            {
                return _significance;
            }
            set
            {
                _significance = value;
                _weightFactor /= _sum;
                //MessageBox.Show("camber significance set");
                OnPropertyChanged(_sumName);
                OnPropertyChanged("Significance");
            }
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
