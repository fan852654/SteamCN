using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Result : Form
    {
        Dictionary<int, string> openWith = new Dictionary<int, string>();
        public Result(int ErrorCode)
        {
            InitializeComponent();
            switch(ErrorCode)
            {
                case 0:
                    textBox1.Text = "成功";
                    break;
                case 100001:
                    textBox1.Text = "你已经成功的选择文件了，不需要继续选择，或者可以清除从新选择";
                    break;
                case 100002:
                    textBox1.Text = "未选择任何文件，请选择";
                    break;
                case 100003:
                    textBox1.Text = "清除数据在选择此项";
                    break;
                case 100005:
                    textBox1.Text = "请不要选择空文件或者关闭选择窗口";
                    break;
                case 100006:
                    textBox1.Text = "请选择目标文件为Steam.exe的快捷方式";
                    break;
                case 100007:
                    textBox1.Text = "请选择Steam启动的可执行文件,Steam.exe";
                    break;
                case 200001:
                    textBox1.Text = "网络连接出现问题，请检查";
                    break;
                case 300000:
                    textBox1.Text = "已经获取了管理员权限";
                    break;
                case 300001:
                    textBox1.Text = "为获取管理员权限不能更改文件，或者延迟问题,请重试";
                    break;
                default:
                    textBox1.Text = "异常代码，联系制作者并提供复现";
                    break;
            }
        }

        private void Result_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
