using Microsoft.Win32;
using System;
using System.Collections;
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
        public Dictionary<string, string> PercentageDic { get; set; }


        public class AP
        {
            public string AP_Name { get; set; }
            public Dictionary<string, string> Battery { get; set; }
            public string SelectedIndex { get; set; }
            public string CBIndex { get; set; }

        }


        string[] UpdateWaiting = new string[1];
        


        public MainWindow()
        {
            UpdateWaiting[0] = "INIT";

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
            RescanAPList();
            DataContext = this;

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


        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {


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
                bool isChanged = false;
                for (int i = 1; i < UpdateWaiting.Length; i++)
                {
                    //ターゲットの行でなければ、飛ばさずWriteLineする
                    if (n.ToString() == UpdateWaiting[i].Substring(0, UpdateWaiting[i].IndexOf(".")))
                    {
                        
                        sw.WriteLine(line.Substring(0, line.IndexOf(",")) + "," + UpdateWaiting[i].Substring(UpdateWaiting[i].IndexOf(".") + 1));
                        Debug.Print(line.Substring(0, line.IndexOf(",")) + "," + UpdateWaiting[i].Substring(UpdateWaiting[i].IndexOf(".") + 1));
                        isChanged = true;
                        break;
                    }
                }
                if(!isChanged) sw.WriteLine(line);
                n++;

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

            UpdateWaiting = new string[1];
            UpdateWaiting[0] = "INIT";

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

            int index = 0;
            while (sr.EndOfStream == false)
            {
                string line = sr.ReadLine();
                string APName = line.Remove(line.IndexOf(","));
                string Battery = line.Remove(0, line.IndexOf(",") + 1);


                Dictionary<string, string> PercentageAndIndex = new Dictionary<string, string>()
            {
                { index.ToString() + ".100", "Always" },
                { index.ToString() + ".90", "90%" },
                { index.ToString() + ".80", "80%" },
                { index.ToString() + ".70", "70%" },
                { index.ToString() + ".60", "60%" },
                { index.ToString() + ".50", "50%" },
                { index.ToString() + ".40", "40%" },
                { index.ToString() + ".30", "30%" },
                { index.ToString() + ".20", "20%" },
                { index.ToString() + ".10", "10%" },
                { index.ToString() + ".0", "None" },
            };

                APList.Items.Add(new AP { CBIndex=index.ToString(), AP_Name = APName, Battery = PercentageAndIndex, SelectedIndex = PercentageToIndex[Battery] });
                index++;
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
                string temp = senderComboBox.SelectedItem.ToString();
                temp = temp.Substring(0, temp.IndexOf(",")).Substring(1);
                Array.Resize(ref UpdateWaiting, UpdateWaiting.Length + 1);
                UpdateWaiting[UpdateWaiting.Length - 1] = temp;
            }
            for(int i = 0; i < UpdateWaiting.Length; i++) Debug.Print(UpdateWaiting[i]);
            UpdateB.IsEnabled = true;
        }



        private void APList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (APList.SelectedIndex >= 0) DeleteB.IsEnabled = true;
        }

        private void cmb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void cmb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
