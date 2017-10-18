using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;

namespace OpnUC_Client_Windows
{
    // http://jsdiy.webcrow.jp/software/wpf_ipcargstest/


    /// <summary>
    /// サーバーチャンネル、クライアントチャンネルの生成。リモートオブジェクトの操作。
    /// </summary>
    class IpcManager : IDisposable
    {
        private readonly string ChannelName;
        private const string PortName = "CommandLineArgs";
        private const string RemoteObjectUri = "IpcRemoteObject";
        private IChannel channel;
        private IpcRemoteObject ipcObject;
        private dynamic targetWindow;
        private Action callback;

        //プロパティ
        public bool IsServer { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="channelName">PC内でユニークなチャンネルネーム</param>
        /// <remarks>
        /// PC内の他のアプリが生成したチャンネルと名前が偶然かぶらないよう
        /// </remarks>
        public IpcManager(string channelName)
        {
            ChannelName = channelName;
        }

        /// <summary>
        /// プロセス間通信を確立する
        /// </summary>
        /// <remarks>
        /// まずサーバーチャンネルを生成し、登録を試みる。
        /// 同名チャンネルが登録済みであれば例外が発生するので、続けてクライアントチャンネルを生成、登録する。
        /// ※同名チャンネルが登録済みかの判定が二重起動の判定にもなる。
        /// </remarks>
        /// <param name="targetWindow">クライアント側から制御を戻す先のウインドウ（MainWindowを想定）</param>
        /// <param name="callback">そのウインドウに含まれるコールバック関数</param>
        public void Connect(dynamic targetWindow, Action callback)
        {
            this.targetWindow = targetWindow;
            this.callback = callback;

            try
            {
                //サーバーチャンネルを生成する
                channel = new IpcServerChannel(ChannelName, PortName);
                ChannelServices.RegisterChannel(channel, true);

                //リモートオブジェクトを公開する
                ipcObject = new IpcRemoteObject();
                ipcObject.ClientStarted += Ipc_ClientStarted;   //イベントハンドラを登録する
                RemotingServices.Marshal(ipcObject, RemoteObjectUri);
            }
            catch (RemotingException)
            {
                //クライアントチャンネルを生成する
                channel = new IpcClientChannel();
                ChannelServices.RegisterChannel(channel, true);

                //リモートオブジェクトを取得する
                ipcObject = Activator.GetObject(typeof(IpcRemoteObject), $"ipc://{PortName}/{RemoteObjectUri}") as IpcRemoteObject;

                //コマンドライン引数をサーバーへ渡す
                ipcObject.Args = Environment.GetCommandLineArgs();
                ipcObject.OnClientStarted();    //イベント発生
            }

            IsServer = channel is IpcServerChannel;
        }

        /// <summary>
        /// 2個目の本アプリが起動したことが通知されてきた
        /// </summary>
        /// <remarks>
        /// このメソッドはMainWindowとは別のスレッドで呼び出されるのでTextBoxなどが操作できない。
        /// そのためデリゲートを通し、MainWindowと同じスレッドで処理できるようにする。
        /// </remarks>
        private void Ipc_ClientStarted(object sender, EventArgs e)
        {
            targetWindow.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// クライアント側から送られたコマンドライン引数を受け取る
        /// </summary>
        public string[] ReceiveArgs()
        {
            return ipcObject.Args;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
            }
        }
    }

    /// <summary>
    /// クライアントのコマンドライン引数を格納し、サーバーへ通知するためのリモートオブジェクト。
    /// </summary>
    class IpcRemoteObject : MarshalByRefObject
    {
        //プロパティ
        public string[] Args { get; set; }

        //イベントハンドラ
        //・サーバー側が、呼んで欲しいメソッドをセットする。
        public event EventHandler ClientStarted;

        //コンストラクタ
        public IpcRemoteObject() { }

        //「クライアントが起動した」というイベントを発生させる
        public void OnClientStarted()
        {
            ClientStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}
