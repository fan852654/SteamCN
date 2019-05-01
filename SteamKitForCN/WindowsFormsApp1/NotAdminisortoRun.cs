using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class NotAdminisortoRun : Form
    {
        ChangeSetSteam css = new ChangeSetSteam();
        public NotAdminisortoRun(ChangeSetSteam css)
        {
            InitializeComponent();
            this.css = css;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            css.cannotad = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
