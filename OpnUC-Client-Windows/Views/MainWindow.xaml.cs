using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Prism.Mvvm;
using OpnUC_Client_Windows.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace OpnUC_Client_Windows.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow, IView
    {
        /// <summary>
        /// アプリケーション 実行時のパス
        /// </summary>
        private string StartupPath = "";

        public IEnumerable<Models.AddressBook> addressBook = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {

            // アプリケーション 実行時のパスを取得
            string exePath = Environment.GetCommandLineArgs()[0];
            string exeFullPath = System.IO.Path.GetFullPath(exePath);
            this.StartupPath = System.IO.Path.GetDirectoryName(exeFullPath);

            InitializeComponent();

        }

        /// <summary>
        /// メニュー：URLハンドラー登録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUrlHandler_Click(object sender, RoutedEventArgs e)
        {

            string path_exe = String.Format(@"""{0}""", System.Reflection.Assembly.GetExecutingAssembly().Location);

            Library.RunAs.RunElevated(System.IO.Path.Combine(StartupPath, "OpnUC-Client-Windows-RegistryTool.exe"), path_exe, true);

        }

        /// <summary>
        /// メニュー：バージョン情報
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuVersion_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new About();

            dialog.Owner = this;
            dialog.ShowDialog();

        }

        private void menuClose_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Shutdown();

        }

        private void menuOption_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new Options();

            dialog.Owner = this;
            dialog.ShowDialog();

        }
    }
}
