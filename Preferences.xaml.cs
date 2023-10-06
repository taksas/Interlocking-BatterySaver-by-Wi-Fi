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

namespace Interlocking_BatterySaver_by_Wi_Fi_
{
    /// <summary>
    /// Preferences.xaml の相互作用ロジック
    /// </summary>
    public partial class Preferences : Window
    {



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

            this.Owner = owner; //呼び出し元のWindow
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner; //起動時の表示位置を親画面の中央に合わせる
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
            this.DialogResult = false;
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

        }
    }
}
