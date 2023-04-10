using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minimal_BatterySaver_Enabler__with_Wi_Fi_
{
    /// <summary>
    /// AddDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddDialog : Window
    {

        /// <summary>
        /// 編集した入力値を呼び出し元に返すためのプロパティ
        /// </summary>
        string[] APs = new string[3];
        public string[] AddAP { get { return APs; } set { APs = value; } }



        public Dictionary<string, string> PercentageDic { get; set; }


        public AddDialog(Window owner)
        {
            APs[0] = "0";
            APs[1] = "";
            APs[2] = "100";

            PercentageDic = new Dictionary<string, string>()
            {
                { "100", "Always" },
                { "90", "90%" },
                { "80", "80%" },
                { "70", "70%" },
                { "60", "60%" },
                { "50", "50%" },
                { "40", "40%" },
                { "30", "30%" },
                { "20", "20%" },
                { "10", "10%" },
                { "0", "None" },
            };

            InitializeComponent();
            DataContext = this;



            this.Owner = owner; //呼び出し元のWindow
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner; //起動時の表示位置を親画面の中央に合わせる
        }

        private void Button_Click_Now(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            string cmd = "chcp 437 && netsh.exe wlan show interfaces";
            p.StartInfo.Arguments = @"/c " + cmd;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string s = p.StandardOutput.ReadToEnd();
            string s1 = "";


            if (s.Length > 120)    
                /*/ If you use Windows11, you got 
                        "Active code page: 437
                        There is no wireless interface on the system.
                        Hosted network status  : Not available"
                    so you need to set more than 108 charactors.
                /*/
                            {
                s1 = s.Substring(s.IndexOf("SSID"));
                s1 = s1.Substring(s1.IndexOf(":"));
                s1 = s1.Substring(2, s1.IndexOf("\n")).Trim();
                AP_Name.Text = s1;
            } else
            {
                AP_Name.Text = "Could not find AP!";
            }

            
            p.WaitForExit();
        }


        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {

        }
    }
}
