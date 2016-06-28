using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Poly3D.Control
{
    public sealed class RenderBehavior : INotifyPropertyChanged
    {
        // Fields...
        private bool _CaptureMouse;
        private bool _PauseOnFormDeactivate;
        private bool _PauseOnLostFocus;

        [DefaultValue(false)]
        public bool CaptureMouse
        {
            get { return _CaptureMouse; }
            set
            {
                if (_CaptureMouse == value)
                    return;
                _CaptureMouse = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CaptureMouse"));
            }
        }

        [DefaultValue(false)]
        public bool PauseOnLostFocus
        {
            get { return _PauseOnLostFocus; }
            set
            {
                if (_PauseOnLostFocus == value)
                    return;
                _PauseOnLostFocus = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PauseOnLostFocus"));
            }
        }

        [DefaultValue(true)]
        public bool PauseOnFormDeactivate
        {
            get { return _PauseOnFormDeactivate; }
            set
            {
                if (_PauseOnFormDeactivate == value)
                    return;
                _PauseOnFormDeactivate = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PauseOnFormDeactivate"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RenderBehavior()
        {
            _PauseOnFormDeactivate = true;
            _PauseOnLostFocus = false;
            _CaptureMouse = false;
        }
    }
}
