using System;
using System.Collections;
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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Net.Http;

namespace Interlocking_BatterySaver_by_Wi_Fi_
{
    /// <summary>
    /// Preferences.xaml の相互作用ロジック
    /// </summary>
    public partial class Preferences : Window
    {

        bool boolDialogResult = false;

        public Preferences(Window owner)
        {


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

            ToggleSwitch_PrefetchMainWindow.IsChecked = Properties.Settings.Default.PrefetchMainWindow;
            ToggleSwitch_DeactivatedWindowClose.IsChecked = Properties.Settings.Default.DeactivatedWindowClose;

            this.Owner = owner; //呼び出し元のWindow
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner; //起動時の表示位置を親画面の中央に合わせる


            Resource_Check(); // リソースチェック開始
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

        private void Button_Click_Help(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://taksas.net/interlocking-batterysaver-by-wi-fi");
        }

        private void Button_Click_Store(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://apps.microsoft.com/store/detail/interlocking-batterysaver-by-wifi/9NZV3DKCLW2P?launch=true&mode=mini");
        }

        private void Button_Click_GitHub(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://github.com/taksas/Interlocking-BatterySaver-by-Wi-Fi");
        }



        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.DialogResult = boolDialogResult;
            this.Close();
        }

        private void Config_MainWindowPrefetch_Changed(object sender, RoutedEventArgs e)
        {


            if ((bool)(sender as ToggleButton).IsChecked)
            {
                Properties.Settings.Default.PrefetchMainWindow = true;
            }
            else
            {
                Properties.Settings.Default.PrefetchMainWindow = false;
            }

            Properties.Settings.Default.Save();
            boolDialogResult = true;
        }

        private void Config_DeactivatedWindowClose_Changed(object sender, RoutedEventArgs e)
        {


            if ((bool)(sender as ToggleButton).IsChecked)
            {
                Properties.Settings.Default.DeactivatedWindowClose = true;
            }
            else
            {
                Properties.Settings.Default.DeactivatedWindowClose = false;
            }

            Properties.Settings.Default.Save();
            boolDialogResult = true;
        }



















        // RESOURCE CHECK


        private async void Resource_Check()
        {
            await Task.Delay(1000);
            Resource_Check_AppPackage();
            await Task.Delay(500);
            Resource_Check_AppVersion();
        }










        // APP_PACKAGE
        private void Resource_Check_AppPackage()
        {

            // Script Definitions
            string SCRIPT_VERSION = "2023/10/08_1";
            string APP_NAME = "InterlockingBatterySaverbyWi-Fi";


            // Set Definitions in GUI
            ResourceCheck_AppPackage_Script_Date.Text = SCRIPT_VERSION;
            ResourceCheck_AppPackage_Script_Target_Name.Text = APP_NAME;


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            string cmd = "powershell \" Get-AppxPackage -Name *" + APP_NAME + "* \"";
            p.StartInfo.Arguments = @"/c " + cmd;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string s = p.StandardOutput.ReadToEnd();


            ResourceCheck_AppPackage_Ring.Visibility = Visibility.Collapsed;
            ResourceCheck_AppPackage_Script_Result.Text = CountChar(s, "PackageFullName").ToString();
            if (CountChar(s, "PackageFullName") == 1)
            {
                ResourceCheck_AppPackage_Ring_Text.Text = Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.ResourceCheck_Success;
                ResourceCheck_AppPackage_Success.Visibility = Visibility.Visible;
            }
            else
            {
                ResourceCheck_AppPackage_Ring_Text.Text = Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.ResourceCheck_Failed;
                ResourceCheck_AppPackage_Failed.Visibility = Visibility.Visible;
            }



        }

        public static int CountChar(string s, string c)
        {
            return (s.Length - s.Replace(c.ToString(), "").Length) / c.Length;
        }











        // APP_VERSION

        private async void Resource_Check_AppVersion()
        {
            // Script Definitions
            string SCRIPT_VERSION = "2023/10/09_1";


            // Set Definitions in GUI
            ResourceCheck_AppVersion_Script_Date.Text = SCRIPT_VERSION;


            string local_ver = Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.SYSTEM_APP_TITLE.Replace("Interlocking BatterySaver by Wi-Fi  v", "");
            ResourceCheck_AppVersion_Local_Version.Text = local_ver;

            string remote_ver;
            var client = new HttpClient();
            var uri = "https://taksas.net/wp-json/acf/v3/pages/1152";

            try
            {
                var result = await client.GetStringAsync(uri);
                if(result.Contains("ibsbw_app_version")) remote_ver = result.Replace("{\"acf\":{\"ibsbw_app_version\":\"", "").Replace("\"}}", "");
                else remote_ver = "ERROR!";
            } catch
            {
                remote_ver = "ERROR!";
            }
            ResourceCheck_AppVersion_Store_Version.Text = remote_ver;



            ResourceCheck_AppVersion_Ring.Visibility = Visibility.Collapsed;
            if (remote_ver == "ERROR!")
            {
                ResourceCheck_AppVersion_Ring_Text.Text = Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.ResourceCheck_Error;
                ResourceCheck_AppVersion_Error.Visibility = Visibility.Visible;
            } else {
                string[] rv = remote_ver.Split("."), lv = local_ver.Split(".");
                int rv_me = Int32.Parse(rv[0]), rv_mi = Int32.Parse(rv[1]), rv_pa = Int32.Parse(rv[2]), lv_me = Int32.Parse(lv[0]), lv_mi = Int32.Parse(lv[1]), lv_pa = Int32.Parse(lv[2]);
                int lv_all = lv_me * 10000000 + lv_mi * 10000 + lv_pa, rv_all = rv_me * 10000000 + rv_mi * 10000 + rv_pa;
                if (lv_all >= rv_all)
                {
                    ResourceCheck_AppVersion_Ring_Text.Text = Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.ResourceCheck_Success;
                    ResourceCheck_AppVersion_Success.Visibility = Visibility.Visible;
                } else
                {
                    ResourceCheck_AppVersion_Ring_Text.Text = Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.ResourceCheck_Failed;
                    ResourceCheck_AppVersion_Failed.Visibility = Visibility.Visible;
                }
            }

        }

    }
}
