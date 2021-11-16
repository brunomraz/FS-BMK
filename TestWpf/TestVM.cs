using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestWpf
{
    public class TestVM : INotifyPropertyChanged
    {
        private float _camber = 2f;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public float Camber
        {
            get { return _camber; }
            set
            {
                _camber = value;
                MessageBox.Show("prop");
                OnPropertyRaised("Camber");
            }
        }
    }
}
