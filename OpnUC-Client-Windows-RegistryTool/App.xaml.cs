using System;
using System.Windows;

namespace OpnUC_Client_Windows_RegistryTool
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            if (e.Args.Length > 0)
            {
                string path_exe = String.Format(@"{0}", e.Args[0]);

                RegisterUriSchemeShellHandler("tel", "Tel", path_exe);
                MessageBox.Show("レジストリ登録が完了しました。");
            }

            this.Shutdown();

        }

        /// <summary>
        /// URLスキームを登録する
        /// </summary>
        /// <param name="scheme">スキーム</param>
        /// <param name="proto">プロトコル</param>
        /// <param name="path_exe">実行ファイル</param>
        /// <param name="path_icon">アイコン</param>
        private static void RegisterUriSchemeShellHandler(string scheme, string proto, string path_exe, string path_icon = null)
        {

            // https://pf-j.sakura.ne.jp/program/winreg/local.htm
            // http://www.officedaytime.com/tips/vistaprogram.htm

            path_icon = path_icon ?? path_exe;

            var KeyName = Microsoft.Win32.Registry.LocalMachine
                .CreateSubKey("SOFTWARE")
                .CreateSubKey("OpnUC")
                .CreateSubKey("Client")
                .CreateSubKey("Capabilities")
                .ToString();

            Microsoft.Win32.Registry.LocalMachine
                .CreateSubKey("SOFTWARE")
                .CreateSubKey("OpnUC")
                .CreateSubKey("Client")
                .CreateSubKey("Capabilities")
                .SetValue("ApplicationName", "OpnUC Client");

            Microsoft.Win32.Registry.LocalMachine
                .CreateSubKey("SOFTWARE")
                .CreateSubKey("OpnUC")
                .CreateSubKey("Client")
                .CreateSubKey("Capabilities")
                .SetValue("ApplicationDescription", "OpnUC Client");

            Microsoft.Win32.Registry.LocalMachine
                .CreateSubKey("SOFTWARE")
                .CreateSubKey("OpnUC")
                .CreateSubKey("Client").CreateSubKey("Capabilities")
                .CreateSubKey("URLAssociations")
                .SetValue("tel", "OpnUC-Client.Url.tel");

            Microsoft.Win32.Registry.LocalMachine
                .CreateSubKey("SOFTWARE")
                .CreateSubKey("RegisteredApplications")
                .SetValue("OpnUC", "SOFTWARE\\OpnUC\\Client\\Capabilities");

            var url = Microsoft.Win32.Registry.ClassesRoot
                .CreateSubKey("OpnUC-Client.Url.tel");

            url.SetValue("", string.Format("URL:{0} Protocol", proto));
            url.SetValue("URL Protocol", "");
            var key_deficon = url.CreateSubKey("DefaultIcon");
            key_deficon.SetValue("", string.Format(@"""{0}""", path_icon));

            // Executable Path
            var key_command = url.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
            key_command.SetValue("",
                string.Format(@"""{0}"" ""%1""", path_exe)
                );

        }
              
    }

}
