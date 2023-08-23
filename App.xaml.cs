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
using System.Windows.Controls;
using System.Threading;

namespace Interlocking_BatterySaver_by_Wi_Fi_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        



        //常駐終了時に開放するために保存しておく
        private System.Windows.Forms.NotifyIcon _notifyIcon;


        bool shutdown = false;
        bool isCreatingMainWindow = false;
        bool APDetectGate = true;



        string[] Percentage = {"100", "90", "80", "70", "60", "50", "40", "30", "20", "10", "0"};



        MainWindow _win = null;


        /// <summary>
        /// 常駐開始時の初期化処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //継承元のOnStartupを呼び出す
            base.OnStartup(e);


            


            //アイコンの取得
            var icon = GetResourceStream(new Uri("leaf_20579.ico", UriKind.Relative)).Stream;



            //通知領域にアイコンを表示
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "Interlocking BatterySaver by Wi-Fi"
            };

            //アイコンがクリックされたら設定画面を表示
            _notifyIcon.MouseClick += (s, er) =>
            {
                
                    ShowMainWindow();
                
            };



            



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

            


            

            

                
            NetworkChange.NetworkAddressChanged += (s, e) => NetworkChange_NetworkAvailabilityChanged(s, e) ;

            

            ExecuteMainFunc();

            Deactivated += ((obj, ev) => {
                if (!shutdown && !isCreatingMainWindow)
                {
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }
            });

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

        public void AppExitFunc()
        {
            shutdown = true;
            System.Windows.Application.Current.Shutdown();
        }




        /// <summary>
        /// 設定画面を表示
        /// </summary>
        private void ShowMainWindow()
        {
            isCreatingMainWindow = true;
            Debug.Print("SHOWING MAIN WINDOW...");
                _win = new MainWindow(this);


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


            isCreatingMainWindow = false;
            //Windowsを表示する
            _win.Show();
            ExecuteMainFunc();
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



        public void ExecuteMainFunc()
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
                    TriggeredInfoChange(temp1, temp2);
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
            TriggeredInfoChange(Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.When_not_connected_to_WiFi, Percentage[Interlocking_BatterySaver_by_Wi_Fi_.Properties.Settings.Default.NotConnected]);
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
            TriggeredInfoChange(Interlocking_BatterySaver_by_Wi_Fi_.Properties.Resources.When_connected_to_other_WiFi, Percentage[Interlocking_BatterySaver_by_Wi_Fi_.Properties.Settings.Default.OtherConnected]);
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


        private void TriggeredInfoChange(string AP, string PercentageStr)
        { 
            if (_win == null) return;
            _win.Dispatcher.Invoke(() => {
                _win.TriggeredInfo_textBlock.Text = AP;
                _win.TriggeredInfo_Percentage_textBlock.Text = _win.PercentageDic[PercentageStr];
            } );
        }
    }
}
