using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrcTest.UI
{
    public partial class ctrlFloatController : UserControl, IController<float>
    {
        public float Value
        {
            get { return ( float )numValue.Value; }

            set { numValue.Value = ( decimal )value; }
        }

        public ctrlFloatController( float min, float max )
        {
            InitializeComponent();
            numValue.Minimum = ( decimal )min;
            numValue.Maximum = ( decimal )max;
        }

        public ctrlFloatController()
            : this( 0, 1 )
        { }
    }
}
