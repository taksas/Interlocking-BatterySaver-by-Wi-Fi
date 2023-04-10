using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minimal_BatterySaver_Enabler__with_Wi_Fi_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> PercentageDic { get; set; }


        


        public MainWindow()
        {
            PercentageDic  = new Dictionary<string, string>()
            {
                { "100", "Always" },
                { "50", "50%" },
                { "40", "40%" },
                { "30", "30%" },
                { "20", "20%" },
                { "10", "10%" },
                { "0", "None" },
            };

            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {/*
            // Process オブジェクトを生成
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;

            string cmd = " powercfg /setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD 100 && powercfg /setactive scheme_current";
            p.StartInfo.Arguments = @"/c " + cmd;
            //起動
            p.Start();
            */
            new AddDialog(this).ShowDialog();
            string[] temp = AddDialog.AddAP;
            Title.Text = ;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Process オブジェクトを生成
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;

            string cmd = " powercfg /setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD "+ cmb.SelectedValue.ToString() + " && powercfg /setactive scheme_current";
            p.StartInfo.Arguments = @"/c " + cmd;
            //起動
            p.Start();
        }
    }
}
