using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MahApps.Metro.Controls;

namespace OpnUC_Client_Windows.Views
{
    /// <summary>
    /// About.xaml の相互作用ロジック
    /// </summary>
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();

            this.DataContext = this;
            this.FileVersionInfo =
                FileVersionInfo.GetVersionInfo(Environment.GetCommandLineArgs()[0]);
        }

        public FileVersionInfo FileVersionInfo
        {
            get { return (FileVersionInfo)GetValue(FileVersionInfoProperty); }
            set { SetValue(FileVersionInfoProperty, value); }
        }

        public static readonly DependencyProperty FileVersionInfoProperty =
            DependencyProperty.Register("FileVersionInfo", typeof(FileVersionInfo),
                typeof(About), new UIPropertyMetadata(null));

        /// <summary>
        /// 閉じるボタンを押した時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }
    }
}