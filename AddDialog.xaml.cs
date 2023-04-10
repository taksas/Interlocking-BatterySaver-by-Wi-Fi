using System;
using System.Collections.Generic;
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

namespace Minimal_BatterySaver_Enabler__with_Wi_Fi_
{
    /// <summary>
    /// AddDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddDialog : Window
    {

        /// <summary>
        /// 編集した入力値を呼び出し元に返すためのプロパティ
        /// </summary>
        string[] APs = new string[3];
        APs[0] = "0";
        APs[1] = "";
        APs[2] = "100";
        public string[] AddAP { get { return APs; } set { APs = value; } }



        public Dictionary<string, string> PercentageDic { get; set; }


        public AddDialog(Window owner)
        {
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
            DataContext = this;



            this.Owner = owner; //呼び出し元のWindow
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner; //起動時の表示位置を親画面の中央に合わせる
        }

        private void Button_Click_Now(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {

        }
    }
}
