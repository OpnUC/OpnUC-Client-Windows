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

            // 画面を閉じてもアプリケーションを終了しないようにする
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            // タスクトレイアイコンを生成
            this.notifyIcon = new NotifyIconWrapper();

            // IPC確立・兼・二重起動確認
            ipcMgr = new IpcManager(AppGuid);
            ipcMgr.Connect(this, IpcCallback_ClientStarted);

            if (ipcMgr.IsServer)
            {
                var args = System.Environment.GetCommandLineArgs();

                if (args.Length > 1)
                {
                    Library.SharedFnc.OnCall(args[1]);
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

            // タスクトレイアイコンを破棄
            if(this.notifyIcon != null)
            {
                this.notifyIcon.Dispose();
            }

        }

        /// <summary>
        /// 重複起動したアプリケーションから呼ばれてる関数
        /// </summary>
        private void IpcCallback_ClientStarted()
        {
            // コマンドライン引数を取得
            var args = ipcMgr.ReceiveArgs();

            // 発信処理
            Library.SharedFnc.OnCall(args[1]);
        }
      

    }
}
