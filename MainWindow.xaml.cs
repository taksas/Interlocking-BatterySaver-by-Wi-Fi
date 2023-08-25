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
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using static System.Windows.Forms.LinkLabel;
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
        App app_origin = null;

        public class AP
        {
            public string AP_Name { get; set; }
            public Dictionary<string, string> Battery { get; set; }
            public string SelectedIndex { get; set; }
            public string CBIndex { get; set; }

        }


        Dictionary<string, int> DoINeedUpdateAPList = new Dictionary<string, int>();



        public bool IsMainWindowLoaded = false;



        public MainWindow(App app_origin_imported)
        {
            app_origin = app_origin_imported;


            PercentageDic = new Dictionary<string, string>()
            {
                { "100", Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.Always },
                { "90", "90%" },
                { "80", "80%" },
                { "70", "70%" },
                { "60", "60%" },
                { "50", "50%" },
                { "40", "40%" },
                { "30", "30%" },
                { "20", "20%" },
                { "10", "10%" },
                { "0", Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.None },
            };

            Initialize_DoINeedUpdateAPList();

            InitializeComponent();
            Wpf.Ui.Appearance.Accent.ApplySystemAccent();

            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.Watcher.Watch(
                    this,                                  // Window class
                    Wpf.Ui.Appearance.BackgroundType.Acrylic, // Background type
                    true                                   // Whether to change accents automatically
                );
            };


            RescanAPList();
            DataContext = this;


        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            app_origin.AppExitFunc();
        }


        private void ExitApp_Button_Click(object sender, RoutedEventArgs e)
        {
            ExitFlyout.Show();
        }


        private void AddAP_Button_Click(object sender, RoutedEventArgs e)
        {
            new AddDialog(this).ShowDialog();
            RescanAPList();

            MW_APList_Snackbar.Show(
                Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SNACKBAR_AddCompletedTitle,
                Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SNACKBAR_AddCompleted,
                SymbolRegular.CheckmarkCircle24,
                Wpf.Ui.Common.ControlAppearance.Secondary
            );
        }



        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            int id = APList.SelectedIndex;
            if (id < 0) return;

            DeleteB.IsEnabled = false;


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


            MW_APList_Snackbar.Show(
                Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SNACKBAR_DeleteCompletedTitle,
                Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SNACKBAR_DeleteCompleted,
                SymbolRegular.Delete24,
                Wpf.Ui.Common.ControlAppearance.Secondary
            );

        }


        private void Initialize_DoINeedUpdateAPList()
        {
            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = System.IO.Path.Combine(roamingDirectory, "IBSbW\\data.txt");
            //ファイルを読み込みで開く
            System.IO.StreamReader sr = new System.IO.StreamReader(filePath);


            int n = 0;
            //内容を一行ずつ読み込む
            while (sr.Peek() > -1)
            {
                //一行読み込む
                string line = sr.ReadLine();
                DoINeedUpdateAPList.Add((n++).ToString(), Int32.Parse(line.Substring(line.IndexOf(",") + 1)));

            }

            sr.Close();
        }


        private void APList_Update_Func(string update_target)
        {

            // 重複時ファイル書き込みしない
            string update_target_index = update_target.Substring(0, update_target.IndexOf("."));
            int update_target_value = Int32.Parse(update_target.Substring(update_target.IndexOf(".") + 1, update_target.Length-2));
            if (DoINeedUpdateAPList.ContainsKey(update_target_index))
            {
                if (DoINeedUpdateAPList[update_target_index] == update_target_value)
                {
                    return;
                }
                else
                {
                    DoINeedUpdateAPList[update_target_index] = update_target_value;
                }
            }
            else
            {
                DoINeedUpdateAPList.Add(update_target_index, update_target_value);
                return;
            }




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
                if (n.ToString() == update_target.Substring(0, update_target.IndexOf(".")))
                {
                    sw.WriteLine(line.Substring(0, line.IndexOf(",")) + "," + update_target.Substring(update_target.IndexOf(".") + 1));
                    // Debug.Print(line.Substring(0, line.IndexOf(",")) + "," + update_target.Substring(update_target.IndexOf(".") + 1));


                } else sw.WriteLine(line);
                n++;

            }
            //閉じる
            sr.Close();
            sw.Close();

            //一時ファイルと入れ替える
            System.IO.File.Copy(tmpPath, filePath, true);
            System.IO.File.Delete(tmpPath);


            DoINeedUpdateAPList.Add(update_target, 1);


            RescanAPList();
            Snackbar_UpdateSuccess_Show();
            
        }


        public void RescanAPList()
        {


            if (app_origin != null)  app_origin.ExecuteMainFunc();
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
                { index.ToString() + ".100", Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.Always },
                { index.ToString() + ".90", "90%" },
                { index.ToString() + ".80", "80%" },
                { index.ToString() + ".70", "70%" },
                { index.ToString() + ".60", "60%" },
                { index.ToString() + ".50", "50%" },
                { index.ToString() + ".40", "40%" },
                { index.ToString() + ".30", "30%" },
                { index.ToString() + ".20", "20%" },
                { index.ToString() + ".10", "10%" },
                { index.ToString() + ".0", Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.None },
            };

                APList.Items.Add(new AP { CBIndex=index.ToString(), AP_Name = APName, Battery = PercentageAndIndex, SelectedIndex = PercentageToIndex[Battery] });
                index++;
            }
            sr.Close();


        }




        

        private void Help_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://taksas.net/interlocking-batterysaver-by-wi-fi");
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


            if ( senderComboBox != null && senderComboBox.SelectedItem != null)
            {
                string temp = senderComboBox.SelectedItem.ToString();
                temp = temp.Substring(0, temp.IndexOf(",")).Substring(1);


                APList_Update_Func(temp);
            }

        }



        private void APList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (APList.SelectedIndex >= 0) DeleteB.IsEnabled = true;
        }

        private void cmb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox senderComboBox = (System.Windows.Controls.ComboBox)sender;
            if (senderComboBox != null)
            {
                Properties.Settings.Default.NotConnected = senderComboBox.SelectedIndex;
                Properties.Settings.Default.Save();
                if (app_origin != null) app_origin.ExecuteMainFunc();
                Snackbar_UpdateSuccess_Show();
            }
            }

            private void cmb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox senderComboBox = (System.Windows.Controls.ComboBox)sender;
            
            if (senderComboBox != null)
            {
                Properties.Settings.Default.OtherConnected = senderComboBox.SelectedIndex;
                Properties.Settings.Default.Save();
                if (app_origin != null) app_origin.ExecuteMainFunc();
                Snackbar_UpdateSuccess_Show();
            }

        }

        
        private void Snackbar_UpdateSuccess_Show()
        {
            if (!IsMainWindowLoaded) return;

                MW_APList_Snackbar.Show(
                Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SNACKBAR_UpdateCompletedTitle,
                Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SNACKBAR_UpdateCompleted,
                SymbolRegular.ArrowSyncCheckmark24,
                Wpf.Ui.Common.ControlAppearance.Secondary
            );
        }



    }
}
