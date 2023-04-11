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
using System.IO;

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
                { "100", "100%" },
                { "90", "90%" },
                { "80", "80%" },
                { "70", "70%" },
                { "60", "60%" },
                { "50", "50%" },
                { "40", "40%" },
                { "30", "30%" },
                { "20", "20%" },
                { "10", "10%" },
                { "0", "0%" },
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


            if (s.Length > 120)    
                /*/ If you use Windows11, you got 
                        "Active code page: 437
                        There is no wireless interface on the system.
                        Hosted network status  : Not available"
                    so you need to set more than 108 charactors.
                /*/
                            {
                APs[1] = s.Substring(s.IndexOf("SSID"));
                APs[1] = APs[1].Substring(APs[1].IndexOf(":"));
                APs[1] = APs[1].Substring(2, APs[1].IndexOf("\n")).Trim();
                
            } else
            {
                APs[1] = "Could not find AP!";
            }
            AP_Name.Text = APs[1];

            p.WaitForExit();
        }


        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            APs[1] = AP_Name.Text;
            if (APs[1] != "" && APs[1] != "Could not find AP!")
            {
                Reconfirm_tb.Text = "";
                APs[0] = "1";
                APs[2] = cmb.SelectedValue.ToString();

                var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var filePath = System.IO.Path.Combine(roamingDirectory, "MBSEwW\\data.txt");

                // FileInfoのインスタンスを生成する
                FileInfo fileInfo = new FileInfo(filePath);

                // フォルダーが存在するかどうかを確認
                if (!fileInfo.Directory.Exists)
                {
                    // フォルダーが存在しない場合は作成
                    fileInfo.Directory.Create();
                }
                // ファイルが存在するかどうかを確認
                if (!File.Exists(filePath))
                {
                    fileInfo.Create().Close();
                }
                


                File.AppendAllText(filePath, APs[1] + "," + APs[2] + "\n");




                this.Close();
            }
            else
            {
                Reconfirm_tb.Text = "Invalid AP name is set. Please reconfirm.";
            }
        }
    }
}
