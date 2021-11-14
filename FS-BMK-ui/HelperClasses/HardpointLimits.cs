using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS_BMK_ui.HelperClasses
{
    public class HardpointLimits
    {
        private string _hardpointName;
        private float _xValLow;
        private float _yValLow;
        private float _zValLow;
        private float _xValHigh;
        private float _yValHigh;
        private float _zValHigh;
        private bool _xIsEditable;
        private bool _yIsEditable;
        private bool _zIsEditable;

        public HardpointLimits(string hardpointName,
            float xValLow, float xValHigh,
            float yValLow, float yValHigh,
            float zValLow, float zValHigh,
            bool xIsEditable,
            bool yIsEditable,
            bool zIsEditable
            )
        {
            HardpointNameClass = hardpointName;
            XValLow = xValLow;
            YValLow = yValLow;
            ZValLow = zValLow;
            XValHigh = xValHigh;
            YValHigh = yValHigh;
            ZValHigh = zValHigh;
            _xIsEditable = xIsEditable;
            _yIsEditable = yIsEditable;
            _zIsEditable = zIsEditable;

        }


        public bool XIsEditable { get { return _xIsEditable; } }
        public bool YIsEditable { get { return _yIsEditable; } }
        public bool ZIsEditable { get { return _zIsEditable; } }

        public string HardpointNameClass
        {
            get { return _hardpointName; }
            set { _hardpointName = value; }
        }

        public float XValLow
        {
            get { return _xValLow; }
            set { _xValLow = value; }
        }
        public float YValLow
        {
            get { return _yValLow; }
            set { _yValLow = value; }
        }
        public float ZValLow
        {
            get { return _zValLow; }
            set { _zValLow = value; }
        }
        public float XValHigh
        {
            get { return _xValHigh; }
            set { _xValHigh = value; }
        }
        public float YValHigh
        {
            get { return _yValHigh; }
            set { _yValHigh = value; }
        }
        public float ZValHigh
        {
            get { return _zValHigh; }
            set { _zValHigh = value; }
        }

    }

}
