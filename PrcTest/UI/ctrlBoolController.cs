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
    public partial class ctrlBoolController : UserControl, IController<bool>
    {
        public ctrlBoolController()
        {
            InitializeComponent();
        }

        public bool Value
        {
            get { return cbValue.Checked; }

            set { cbValue.Checked = value; }
        }
    }
}
