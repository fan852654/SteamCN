using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class NeedConf : Form
    {
        public NeedConf()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(File.Exists("confforip.txt"))
            {
                File.Delete("confforip.txt");
            }
            FileStream fs = new FileStream("confforip.txt", FileMode.CreateNew);
            StreamWriter twer = new StreamWriter(fs);
            twer.WriteLine("104.115.227.3" + "\r\n");
            twer.WriteLine("104.74.243.84" + "\r\n");
            twer.WriteLine("23.66.253.192" + "\r\n");
            twer.WriteLine("23.37.147.226" + "\r\n");
            twer.WriteLine("118.214.249.13" + "\r\n");
            twer.WriteLine("23.222.161.85" + "\r\n");
            twer.WriteLine("23.50.18.229" + "\r\n");
            twer.Flush();
            twer.Close();
            fs.Close();
            Close();
        }

        private void NeedConf_Load(object sender, EventArgs e)
        {

        }
    }
}
