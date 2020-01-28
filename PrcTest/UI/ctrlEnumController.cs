using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrcTest.UI;
using System.Reflection;

namespace PrcTest.UI
{
    public partial class ctrlEnumController : UserControl, IController<Enum>
    {
        private Type _enumType = null;

        public ctrlEnumController()
        {
            InitializeComponent();
        }

        public Enum Value
        {
            get
            {
                return (Enum)Enum.Parse(_enumType, ddValues.Text);
            }

            set
            {
                _enumType = value.GetType();

                ddValues.Items.AddRange(Enum.GetNames(_enumType));
                ddValues.Text = Enum.GetName(_enumType, value);
            }
        }
    }
}
