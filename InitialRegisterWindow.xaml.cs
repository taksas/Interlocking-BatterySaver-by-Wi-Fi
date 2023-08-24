using System;
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

namespace Interlocking_BatterySaver_by_Wi_Fi_
{
    /// <summary>
    /// AddDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InitialRegisterWindow : Window
    {




        public InitialRegisterWindow()
        {
            

            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
