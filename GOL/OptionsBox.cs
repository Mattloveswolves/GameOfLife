using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL
{
    public partial class OptionsBox : Form
    {
        public OptionsBox()
        {
            InitializeComponent();
        }
        // Makes the values accessible outside of class
        public decimal Timer 
        { 
            get { return numericUpDownTime.Value; } 
            set { numericUpDownTime.Value = value; ; } 
        }
        public decimal width
        {
            get { return numericUpDownWidth.Value; }
            set { numericUpDownWidth.Value = value; ; }
        }
        public decimal height
        {
            get { return numericUpDownHeight.Value; }
            set { numericUpDownHeight.Value = value; ; }
        }
    }
}
