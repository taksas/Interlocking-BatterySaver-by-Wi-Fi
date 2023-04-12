using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Interlocking_BatterySaver_by_Wi_Fi_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {



        public class AP
        {
            public string AP_Name { get; set; }
            public string[] Battery { get; set; }
            public string SelectedIndex { get; set; }

        }



        public MainWindow()
        {
            InitializeComponent();
            RescanAPList();

        }




        private void AddAP_Button_Click(object sender, RoutedEventArgs e)
        {
            new AddDialog(this).ShowDialog();
            RescanAPList();
        }



        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            int id = APList.SelectedIndex;
            if (id < 0) return;

            DeleteB.IsEnabled = false;
            UpdateB.IsEnabled = false;


            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = System.IO.Path.Combine(roamingDirectory, "IBSbW\\data.txt");
            //ファイルを読み込みで開く
            System.IO.StreamReader sr = new System.IO.StreamReader(filePath);
            //一時ファイルを作成する
            string tmpPath = System.IO.Path.GetTempFileName();
            //一時ファイルを書き込みで開く
            System.IO.StreamWriter sw = new System.IO.StreamWriter(tmpPath);

            int n = 0;
            //内容を一行ずつ読み込む
            while (sr.Peek() > -1)
            {
                //一行読み込む
                string line = sr.ReadLine();
                //ターゲットの行でなければ、飛ばさずWriteLineする
                if (n++ != id) sw.WriteLine(line);

            }
            //閉じる
            sr.Close();
            sw.Close();

            //一時ファイルと入れ替える
            System.IO.File.Copy(tmpPath, filePath, true);
            System.IO.File.Delete(tmpPath);

            

            RescanAPList();

        }



        private void RescanAPList()
        {
            APList.Items.Clear();


            Dictionary<string, string> PercentageToIndex = new Dictionary<string, string>()
            {
                { "100", "0" },
                { "90", "1" },
                { "80", "2" },
                { "70", "3" },
                { "60", "4" },
                { "50", "5" },
                { "40", "6" },
                { "30", "7" },
                { "20", "8" },
                { "10", "9" },
                { "0", "10" },
            };

            string[] PercentageStringDic =
                new string[] { "Always", "90%", "80%", "70%", "60%", "50%", "40%", "30%", "20%", "10%", "None" };

            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = System.IO.Path.Combine(roamingDirectory, "IBSbW\\data.txt");
            StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8"));

            while (sr.EndOfStream == false)
            {
                string line = sr.ReadLine();
                string APName = line.Remove(line.IndexOf(","));
                string Battery = line.Remove(0, line.IndexOf(",") + 1);
                APList.Items.Add(new AP { AP_Name = APName, Battery = PercentageStringDic, SelectedIndex = PercentageToIndex[Battery] });
            }
            sr.Close();
        }


        private void Help_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://taksas.net");
        }

        private Process OpenUrl(string url)
        {
            ProcessStartInfo pi = new ProcessStartInfo()
            {
                FileName = url,
                UseShellExecute = true,
            };

            return Process.Start(pi);
        }



        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox senderComboBox = (System.Windows.Controls.ComboBox)sender;

            // Change the length of the text box depending on what the user has 
            // selected and committed using the SelectionLength property.
            if (senderComboBox != null)
            {
                
                UpdateB.IsEnabled = true;
            }
        }

        private void APList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (APList.SelectedIndex >= 0) DeleteB.IsEnabled = true;
        }
    }
}
