using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ChangeSetSteam : Form
    {
        public ChangeSetSteam()
        {
            InitializeComponent();
        }
       
        string filepath = "";
        Dictionary<string, long> ipandtime = new Dictionary<string, long>();

        private void Form1_Load(object sender, EventArgs e)
        {
            FreeZeren fz = new FreeZeren();
            fz.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(filepath != "")
            {
                Result r = new Result(ErrorCode:10001);
                r.Show();
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = @"Steam|*.exe"
            };
            ofd.ShowDialog();
            filepath = ofd.FileName.ToString();
            ofd.Dispose();
            if(!filepath.Contains("Steam.exe"))
            {
                Result rst = new Result(ErrorCode: 100007);
                rst.Show();
                filepath = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                Result rs = new Result(ErrorCode:100001);
                rs.Show();
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = @"Steam|*.lnk"
            };
            ofd.ShowDialog();
            filepath = ofd.FileName.ToString();
            ofd.Dispose();
            if(filepath == "")
            {
                Result rss = new Result(ErrorCode: 100005);
                rss.Show();
                return;
            }
            string argu = GetMorePowerSteam() + " -community=\"https://steamcommunity.com\"";
            WshShell shell = new WshShell();
            IWshShortcut iws = (IWshShortcut)shell.CreateShortcut(filepath);
            string targetpath = iws.TargetPath;
            if(!targetpath.Contains("Steam.exe"))
            {
                Result rt = new Result(ErrorCode: 100006);
                rt.Show();
                targetpath = "";
                filepath = "";
                return;
            }
            System.IO.File.Delete(filepath);
            WshShell was = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(filepath);//创建快捷方式对象
            shortcut.TargetPath = targetpath;//指定目标路径
            shortcut.Arguments = argu;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetpath);//设置起始位置
            shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
            shortcut.Description = "可用Steam连接文件";//设置备注
            shortcut.IconLocation = targetpath;
            shortcut.Save();//保存快捷方式
            Result r = new Result(ErrorCode:0);
            r.Show();
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            filepath = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(filepath == "")
            {
                Result rs = new Result(ErrorCode:100002);
                rs.Show();
                return;
            }
            if (filepath.Contains("lnk"))
            {
                Result rs = new Result(ErrorCode:100003);
                rs.Show();
                return;
            }
            string argu = GetMorePowerSteam() + " -community=\"https://steamcommunity.com\"";
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//获取桌面文件夹路径
            string newlnk = Path.Combine(desktop, "Steam.lnk");
            if (System.IO.File.Exists(newlnk))
            {
                System.IO.File.Delete(newlnk);
            }
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(newlnk);//创建快捷方式对象
            shortcut.TargetPath = filepath;//指定目标路径
            shortcut.Arguments = argu;
            shortcut.WorkingDirectory = Path.GetDirectoryName(filepath);//设置起始位置
            shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
            shortcut.Description = "可用Steam连接文件";//设置备注
            shortcut.IconLocation = filepath;
            shortcut.Save();//保存快捷方式
            Result r = new Result(ErrorCode:0);
            r.Show();
            return;
        }

        public bool cannotad = false;
        private void button5_Click(object sender, EventArgs e)
        {
            ipandtime.Clear();
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                NotAdminisortoRun nar = new NotAdminisortoRun(this);
                nar.ShowDialog();
                if(!cannotad)
                {
                    return;
                }
            }
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "confforip.txt";
                //获取文件内容
                if (!System.IO.File.Exists(path))
                {
                    NeedConf nc = new NeedConf();
                    nc.Show();
                    return;
                }
                string[] ips = System.IO.File.ReadAllLines(path);
                ips = DelRepeatData(ips);
                ips = DelNullData(ips);
                Thread[] teskping = new Thread[ips.Count()];
                for (int i = 0; i < ips.Count(); i++)
                {
                    Thread sanip = new Thread(new ParameterizedThreadStart(ConnectIP));
                    sanip.Start(ips[i]);
                    teskping[i] = sanip;
                }
                bool threadst = true;
                while (threadst)
                {
                    bool iscon = true;
                    for (int a = 0; a < teskping.Length; a++)
                    {
                        if (teskping[a] == null)
                            continue;
                        while (teskping[a].IsAlive && iscon)
                        {
                            iscon = false;
                        }
                        if (teskping[a].IsAlive == false)
                        {
                            ips[a] = null;
                        }
                    }
                    if (ips.All(m => m == null))
                    {
                        threadst = false;
                    }
                }
                //电脑可能未连接网络成功
                if (ipandtime == null)
                {
                    Result rs = new Result(ErrorCode:100003);
                    rs.Show();
                    ipandtime.Clear();
                    return;
                }
                string needtobeip = ipandtime.First(a => a.Value == ipandtime.Values.Min()).Key;
                if (!System.IO.File.Exists("C:\\Windows\\System32\\drivers\\etc\\hosts"))
                {
                    FileStream fs = new FileStream("C:\\Windows\\System32\\drivers\\etc\\hosts", FileMode.CreateNew);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("\r\n" + needtobeip + "\tsteamcommunity.com");
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                DelsteamcommHost();
                FileStream fss = new FileStream("C:\\Windows\\System32\\drivers\\etc\\hosts", FileMode.Append);
                StreamWriter swer = new StreamWriter(fss);
                swer.WriteLine("\r\n" + needtobeip + "\tsteamcommunity.com");
                swer.Flush();
                swer.Close();
                fss.Close();
                Result r = new Result(ErrorCode:0);
                r.Show();
                return;
            }
            catch
            {
                Result rs = new Result(ErrorCode:300001);
                rs.Show();
                return;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                //创建启动对象
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                System.Diagnostics.Process.Start(startInfo);
                //退出
                Application.Exit();
            }
            else
            {
                Result rs = new Result(ErrorCode:300000);
                rs.Show();
            }
        }

        public void ConnectIP(object ips)
        {
            //远程服务器IP  
            string ipStr = ips.ToString();
            //构造Ping实例  
            Ping pingSender = new Ping();
            //Ping 选项设置  
            PingOptions options = new PingOptions
            {
                DontFragment = true,
            };
            //测试数据  
            string data = "test data abcabc";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            //设置超时时间  
            int timeout = 3;
            //调用同步 send 方法发送数据,将返回结果保存至PingReply实例  
            PingReply reply = pingSender.Send(ipStr, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                long time = reply.RoundtripTime;
                ipandtime.Add(ipStr, time);
            }
        }
        //去除多余的元素
        public static string[] DelRepeatData(string[] array)
        {
            return array.GroupBy(p => p).Select(p => p.Key).ToArray();
        }
        //去除空值元素
        public static string[] DelNullData(string[] array)
        {
            List<string> list = new List<string>();
            foreach (string s in array)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    list.Add(s);
                }
            }
            return list.ToArray();
        }
        //去除带有steamcommunity.com的项目
        public static bool DelsteamcommHost()
        {
            List<string> lines = new List<string>(System.IO.File.ReadAllLines("C:\\Windows\\System32\\drivers\\etc\\hosts"));
            for (int i = 0;i<lines.Count();i++)
            {
                if (lines[i].Contains("steamcommunity.com"))
                {
                    lines.RemoveAt(i);
                }
            }
            System.IO.File.WriteAllLines("C:\\Windows\\System32\\drivers\\etc\\hosts", lines.ToArray());
            return true;
        }
        //获取需要的额外功能
        public string GetMorePowerSteam()
        {
            string cl = string.Empty;
            if (checkBox1.Checked)
                cl = " -cafeapplaunch" + cl;
            if (checkBox2.Checked)
                cl = " -console" + cl ;
            if (checkBox3.Checked)
                cl = " -developer" +cl;
            if (checkBox4.Checked)
                cl = " -silent" + cl;
            if (checkBox5.Checked)
                cl = " -tcp" + cl;
            if (checkBox6.Checked)
                cl = " -single_core" + cl;
            return cl;
        }
    }
}
