using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FS_BMK_ui.HelperClasses
{
    class CurrentSuspCharacteristics : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private float[] _characteristic;

        public float[] Characteristic
        {
            get { return _characteristic; }
            set { _characteristic = value; }
        }

        private int _steerPos;

        public int SteerPos
        {
            get { return _steerPos; }
            set { _steerPos = value; OnPropertyChanged("SteerPos"); }
        }

        private int _vertIncr;

        public int VertIncr
        {
            get { return _vertIncr; }
            set { _vertIncr = value; }
        }

        private int _steerIncr;

        public int SteerIncr
        {
            get { return _steerIncr; }
            set { _steerIncr = value; }
        }

        private float _vertMovement;

        public float VertMovement
        {
            get { return _vertMovement; }
            set { _vertMovement = value; }
        }


        public CurrentSuspCharacteristics(float vertMovement, int vertIncr, int steerIncr, string name, float[] characteristic)
        {
            _name = name;
            _characteristic = characteristic;
            _steerIncr = steerIncr;
            _vertIncr = vertIncr;
            _vertMovement = vertMovement;
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
