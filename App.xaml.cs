using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using System.Reflection;
using System.Buffers.Text;

namespace Interlocking_BatterySaver_by_Wi_Fi_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        //常駐終了時に開放するために保存しておく
        private System.Windows.Forms.ContextMenuStrip _menu;
        private System.Windows.Forms.NotifyIcon _notifyIcon;


        bool shutdown = false;
        bool isCreatingMainWindow = false;
        bool APDetectGate = true;



        string[] Percentage = {"100", "90", "80", "70", "60", "50", "40", "30", "20", "10", "0"};






        /// <summary>
        /// 常駐開始時の初期化処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //継承元のOnStartupを呼び出す
            base.OnStartup(e);

            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = System.IO.Path.Combine(roamingDirectory, "IBSbW\\data.txt");

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


            RegisterStartup();

            //アイコンの取得
            var icon = GetResourceStream(new Uri("leaf_20579.ico", UriKind.Relative)).Stream;

            // コンテキストメニューを作成
            _menu = CreateMenu();

            //通知領域にアイコンを表示
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "Interlocking BatterySaver by Wi-Fi",
                ContextMenuStrip = _menu
            };





            

                
            NetworkChange.NetworkAddressChanged += (s, e) => NetworkChange_NetworkAvailabilityChanged(s, e) ;

            //アイコンがクリックされたら設定画面を表示
            _notifyIcon.MouseClick += (s, er) =>
            {
                if (er.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    //コンテキストメニューを表示する座標
                    System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

                    //指定した画面上の座標位置にコンテキストメニューを表示する
                    if(_notifyIcon != null) _notifyIcon.ContextMenuStrip.Show(p);

                    // ShowMainWindow();
                }
            };



            Deactivated += ((obj, ev) => {
                if (!shutdown && !isCreatingMainWindow)
                {
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }
            });

            ExecuteMainFunc();
        }

        /// <summary>
        /// 常駐終了時の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();

            //継承元の終了イベントを呼び出す
            base.OnExit(e);
        }





        /// <summary>
        /// 設定画面を表示
        /// </summary>
        private void ShowMainWindow()
        {
            isCreatingMainWindow = true;
            Debug.Print("SHOWING MAIN WINDOW...");
                MainWindow _win = new MainWindow();


                /*
                * Windowの表示位置をマニュアル指定
                */
                _win.WindowStartupLocation = WindowStartupLocation.Manual;

                /*
                 * 表示位置(Top)を調整。
                 * 「ディスプレイの作業領域の高さ」-「表示するWindowの高さ」
                 */
                _win.Top = SystemParameters.WorkArea.Height - _win.Height;

                /*
                 * 表示位置(Left)を調整
                 * 「ディスプレイの作業領域の幅」-「表示するWindowの幅」
                 */
                _win.Left = SystemParameters.WorkArea.Width - _win.Width;



                //Windowsを表示する
                _win.Show();

            /*
            //閉じるボタンが押された時のイベント処理を登録
            _win.Closing += (s, e) =>
            {
                System.Windows.Application.Current.Shutdown();
                System.Windows.Forms.Application.Restart();
                //_win.Hide();        //非表示にする
                //e.Cancel = true;    //閉じるをキャンセルする
            };
            */
            Debug.Print("FINISHED SHOWING MAIN WINDOW...");
            isCreatingMainWindow = false;


        }


        /// <summary>
        /// コンテキストメニューの表示
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip CreateMenu()
        {
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add(Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.Settings, null, (s, e) => { ShowMainWindow(); });
            menu.Items.Add(Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.Exit, null, (s, e) => {
                shutdown = true;
                System.Windows.Application.Current.Shutdown();
            });
            return menu;
        }


        //ネットワーク接続状況が変化した
        private void NetworkChange_NetworkAvailabilityChanged(
            object sender, EventArgs e)
        {
            //if (!APDetectGate) return;
            //APDetectGateFunc();
            ExecuteMainFunc();
            
        }


        private async void APDetectGateFunc()
        {
            APDetectGate = false;
            await Task.Delay(1000);
            APDetectGate = true;
        }



        private void ExecuteMainFunc()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            string cmd = "chcp 437 && netsh.exe wlan show interfaces";
            p.StartInfo.Arguments = @"/c " + cmd;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
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
                try
                {
                    string APs = s.Substring(s.IndexOf("SSID"));
                    APs = APs.Substring(APs.IndexOf(":"));
                    APs = APs.Substring(2, APs.IndexOf("\n")).Trim();

                    WiFiMode(APs);
                } catch { 
                    NoWifiMode(); 
                }

            }
            else
            {
                NoWifiMode();
            }
        }


        private void WiFiMode(string APName)
        {


            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = System.IO.Path.Combine(roamingDirectory, "IBSbW\\data.txt");
            //ファイルを読み込みで開く
            System.IO.StreamReader sr = new System.IO.StreamReader(filePath);


            bool otherwifi = true;
            //内容を一行ずつ読み込む
            while (sr.Peek() > -1)
            {
                //一行読み込む
                string line = sr.ReadLine();
                string temp1 = line.Substring(0, line.IndexOf(","));
                string temp2 = line.Substring(line.IndexOf(",")+1);

                if (temp1 == APName)
                {
                    // Process オブジェクトを生成
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
                    p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                    //ウィンドウを表示しないようにする
                    p.StartInfo.CreateNoWindow = true;
                    string cmd = " powercfg /setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD " + temp2 + " && powercfg /setactive scheme_current";
                    p.StartInfo.Arguments = @"/c " + cmd;
                    //起動
                    p.Start();
                    Debug.Print("WiFiMode");
                    otherwifi = false;
                    break;
                }

            }
            if (otherwifi) OtherWifiMode();
            //閉じる
            sr.Close();


            
        }

        private void NoWifiMode() {
            Debug.Print("NoWiFiMode");

            // Process オブジェクトを生成
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;
            string cmd = " powercfg /setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD " + Percentage[Interlocking_BatterySaver_by_Wi_Fi_.Properties.Settings.Default.NotConnected] + " && powercfg /setactive scheme_current";
            p.StartInfo.Arguments = @"/c " + cmd;
            //起動
            p.Start();
        }
        private void OtherWifiMode() {
            Debug.Print("OtherWiFiMode");

            // Process オブジェクトを生成
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;
            string cmd = " powercfg /setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD " + Percentage[Interlocking_BatterySaver_by_Wi_Fi_.Properties.Settings.Default.OtherConnected] + " && powercfg /setactive scheme_current";
            p.StartInfo.Arguments = @"/c " + cmd;
            //起動
            p.Start();
        }






        private void RegisterStartup()
        {
            // スタートアップフォルダにショートカット作成
            //try
            //{
                string aplTitle = "Interlocking BatterySaver by Wi-Fi"; // アプリ名
                // ショートカットのリンク名
                String sMnu = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string lnkFile = sMnu + "\\InterlockingBatterySaverbyWi-Fi_StartUP.bat";

                var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);



                if (File.Exists(lnkFile))
                {
                    Debug.Print("StartUP Already Registered");
                } else
                {
                    System.Windows.Forms.MessageBox.Show(
                        Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.StartupReg,
                        aplTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }


            // WSHファイル作成
                using (StreamWriter w = new StreamWriter(
                        lnkFile, false, System.Text.Encoding.ASCII))
                    {
                        w.WriteLine("cd /d %~dp0 && powershell \"$app = Get-AppxPackage -Name *InterlockingBatterySaverbyWi-Fi* ; if($app -eq $null) { Remove-item .\\InterlockingBatterySaverbyWi-Fi_StartUP.bat } Else { $appname = $app.PackageFamilyName ; $package = $app | Get-AppxPackageManifest ; $id = $package.Package.Applications.Application.Id ; Start-Process shell:AppsFolder\\$appname!$id }\"");
                    }

            


        }
    }
}
