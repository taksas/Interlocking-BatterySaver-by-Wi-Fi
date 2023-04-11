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

namespace Minimal_BatterySaver_Enabler__with_Wi_Fi_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        //常駐終了時に開放するために保存しておく
        private System.Windows.Forms.ContextMenuStrip _menu;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private MainWindow _win = null;  //２重起動防止用

        bool shutdown = false;

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

            // コンテキストメニューを作成
            _menu = CreateMenu();

            //通知領域にアイコンを表示
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "Minimal BS",
                ContextMenuStrip = _menu
            };





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



            //アイコンがクリックされたら設定画面を表示
            _notifyIcon.MouseClick += (s, er) =>
            {
                if (er.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ShowMainWindow();
                }
            };



            Deactivated += ((obj, ev) => {
                if (!shutdown)
                {
                    System.Windows.Application.Current.Shutdown();
                    System.Windows.Forms.Application.Restart();
                }
            });



            //別タスクで監視処理を実行
            // Task.Run(() => /*～～監視処理～～*/); 
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
            if (_win == null)
            {
                _win = new MainWindow();

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
                
            }
            else
            {
                //Windowsを表示する
                _win.Show();
            }
        }


        /// <summary>
        /// コンテキストメニューの表示
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip CreateMenu()
        {
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("Settings", null, (s, e) => { ShowMainWindow(); });
            menu.Items.Add("Exit", null, (s, e) => {
                shutdown = true;
                Shutdown();
            });
            return menu;
        }


    }
}
