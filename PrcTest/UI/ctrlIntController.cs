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
    public partial class ctrlIntController : UserControl, IController<int>
    {
        public int Value
        {
            get { return (int)numValue.Value; }

            set { numValue.Value = value; }
        }

        public ctrlIntController(int min, int max)
        {
            InitializeComponent();
            numValue.Minimum = min;
            numValue.Maximum = max;
        }

        public ctrlIntController()
            : this(Int32.MinValue, Int32.MaxValue)
        { }
    }
}
