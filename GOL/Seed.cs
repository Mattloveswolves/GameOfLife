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
    public partial class Seed : Form
    {
        public Seed()
        {
            InitializeComponent();
        }

        public decimal seed 
        {
            get { return numericUpDownSeed.Value;}
            set { numericUpDownSeed.Value = value;} 
        }

        //randomizes seed 
        private void button1_Click(object sender, EventArgs e)
        {
            Random rng = new Random();
            numericUpDownSeed.Value = rng.Next(1,10000);
        }
    }
}
