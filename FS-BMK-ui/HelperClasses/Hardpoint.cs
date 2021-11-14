using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS_BMK_ui.HelperClasses
{
    public class Hardpoint
    {
        private string _hardpointName;
        private float _x;
        private float _y;
        private float _z;


        public Hardpoint(string hardpointName,
            float xVal, float yVal,
            float zVal)
        {
            HardpointNameClass = hardpointName;
            X = xVal;
            Y = yVal;
            Z = zVal;

        }

        public string HardpointNameClass
        {
            get { return _hardpointName; }
            set { _hardpointName = value; }
        }

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }


    }

}
