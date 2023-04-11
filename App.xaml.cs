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

namespace Minimal_BatterySaver_Enabler__with_Wi_Fi_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        //常駐終了時に開放するために保存しておく
        private System.Windows.Forms.NotifyIcon _notifyIcon;

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
                Text = "Minimal BS"
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


    }
}
