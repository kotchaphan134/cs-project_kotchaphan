using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1
{
    public partial class aboutme : Form
    {
        public aboutme()
        {
            InitializeComponent();
        }

        private void goback_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void aboutme_Load(object sender, EventArgs e)
        {

        }
    }
}
