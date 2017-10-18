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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpnUC_Client_Windows
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string StartupPath = "";

        public MainWindow()
        {
            string exePath = Environment.GetCommandLineArgs()[0];
            string exeFullPath = System.IO.Path.GetFullPath(exePath);
            this.StartupPath = System.IO.Path.GetDirectoryName(exeFullPath);

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path_exe = String.Format(@"""{0}""", System.Reflection.Assembly.GetExecutingAssembly().Location);

            RunElevated(System.IO.Path.Combine(StartupPath, "OpnUC-Client-Windows-RegistryTool.exe"), path_exe, true);

        }

        /// <summary>
        /// 管理者権限が必要なプログラムを起動する
        /// </summary>
        /// <param name="fileName">プログラムのフルパス。</param>
        /// <param name="arguments">プログラムに渡すコマンドライン引数。</param>
        /// <param name="parentForm">親プログラムのウィンドウ。</param>
        /// <param name="waitExit">起動したプログラムが終了するまで待機する。</param>
        /// <returns>起動に成功した時はtrue。
        /// 「ユーザーアカウント制御」ダイアログでキャンセルされた時はfalse。</returns>
        public static bool RunElevated(string fileName, string arguments, bool waitExit)
        {
            //プログラムがあるか調べる
            if (!System.IO.File.Exists(fileName))
            {
                throw new System.IO.FileNotFoundException();
            }

            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo();
            //ShellExecuteを使う。デフォルトtrueなので、必要はない。
            psi.UseShellExecute = true;
            //昇格して実行するプログラムのパスを設定する
            psi.FileName = fileName;
            //動詞に「runas」をつける
            psi.Verb = "runas";
            //子プログラムに渡すコマンドライン引数を設定する
            psi.Arguments = arguments;

            try
            {
                //起動する
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                if (waitExit)
                {
                    //終了するまで待機する
                    p.WaitForExit();
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //「ユーザーアカウント制御」ダイアログでキャンセルされたなどによって
                //起動できなかった時
                return false;
            }

            return true;
        }
    }
}
