using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace OpnUC_Client_Windows
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// タスクトレイに表示するアイコン
        /// </summary>
        private NotifyIconWrapper notifyIcon;

        /// <summary>
        /// Application GUID
        /// </summary>
        private const string AppGuid = "{FE68D3B3-01D9-4D8C-A5E7-2F058CBA911D}";
        private IpcManager ipcMgr;

        /// <summary>
        /// System.Windows.Application.Startup イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();

            Application.Current.Properties["WebAPI"] = new WebAPI();

            //IPC確立・兼・二重起動確認
            ipcMgr = new IpcManager(AppGuid);
            ipcMgr.Connect(this, IpcCallback_ClientStarted);

            if (ipcMgr.IsServer)
            {
                var args = System.Environment.GetCommandLineArgs();

                if (args.Length > 1)
                {
                    onCall(args[1]);
                }
            }
            else
            {
                this.Shutdown();
            }

        }

        /// <summary>
        /// System.Windows.Application.Exit イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.notifyIcon.Dispose();
        }


        //2個目の本アプリ起動により呼び出される
        private void IpcCallback_ClientStarted()
        {
            //コマンドライン引数を利用する何らかの処理へ
            var args = ipcMgr.ReceiveArgs();
            this.onCall(args[1]);
        }

        private void onCall(string telNumber)
        {

            Regex re = new Regex(@"[^0-9]");
            telNumber = re.Replace(telNumber, "");

            if (MessageBox.Show(telNumber + "に発信します。よろしいですか？",
                "確認", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                var a = new WebAPI();
                //a.onCall(telNumber);
            }

        }

    }
}
