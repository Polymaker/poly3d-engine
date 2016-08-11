using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Poly3D.Platform
{
    public class DisplayEngineSettings : INotifyPropertyChanged
    {
        private double _UpdatePeriod;
        private double _RenderPeriod;

        /// <summary>
        /// Gets or sets a double representing the target render frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 200.0Hz are clamped to 200.0Hz.</para>
        /// </remarks>
        [DefaultValue(60.0), RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets a double representing the target render frequency, in hertz.")]
        public double RenderFrequency
        {
            get
            {
                if (RenderPeriod <= 0d)
                    return 0d;
                return 1d / RenderPeriod;
            }
            set
            {
                if (value < 1d)
                    RenderPeriod = 0d;
                else
                    RenderPeriod = 1d / Math.Min(value, 200d);
            }
        }

        /// <summary>
        /// Gets or sets a double representing the target render period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.005 seconds (200Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        [DefaultValue(0.016666666666666666), RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets a double representing the target render period, in hertz.")]
        public double RenderPeriod
        {
            get { return _RenderPeriod; }
            set
            {
                if (_RenderPeriod == value)
                    return;
                _RenderPeriod = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RenderPeriod"));
            }
        }

        /// <summary>
        /// Gets or sets a double representing the target update frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 200.0Hz are clamped to 200.0Hz.</para>
        /// </remarks>
        [DefaultValue(60.0), RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets a double representing the target update frequency, in hertz.")]
        public double UpdateFrequency
        {
            get
            {
                if (UpdatePeriod <= 0d)
                    return 0d;
                return 1d / UpdatePeriod;
            }
            set
            {
                if (value < 1d)
                    UpdatePeriod = 0d;
                else
                    UpdatePeriod = 1d / Math.Min(value, 200d);
            }
        }

        /// <summary>
        /// Gets or sets a double representing the target update period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.005 seconds (200Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        [DefaultValue(0.016666666666666666), RefreshProperties(RefreshProperties.All)]
        [Description("Gets or sets a double representing the target update period, in hertz.")]
        public double UpdatePeriod
        {
            get { return _UpdatePeriod; }
            set
            {
                if (_UpdatePeriod == value)
                    return;
                _UpdatePeriod = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UpdatePeriod"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSettings"/> class.
        /// </summary>
        public DisplayEngineSettings()
        {
            _UpdatePeriod = 0.016666666666666666d;
            _RenderPeriod = 0.016666666666666666d;
        }
    }
}
