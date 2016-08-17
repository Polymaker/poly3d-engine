using Poly3D.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Poly3D.Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var scene1 = Scene.CreateDefault();
            var scene2 = Scene.CreateDefault();
            engineControl1.LoadScene(scene1);
            engineControl2.LoadScene(scene2);
            scene1.Start();
            scene2.Start();
        }
    }
}
