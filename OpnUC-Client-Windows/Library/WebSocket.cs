using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

namespace OpnUC_Client_Windows.Library
{
    public class WebSocket
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Quobject.SocketIoClientDotNet.Client.Socket socket = null;

        #region EventHandler

        /// <summary>
        /// プレゼンス情報の更新を通知するイベントハンドラー
        /// </summary>
        public event UpdatePresenceEventHandler UpdatePresenceEvent;

        /// <summary>
        /// プレゼンス情報の更新
        /// </summary>
        /// <param name="ext">内線番号</param>
        /// <param name="status">状態</param>
        public void UpdateProgress(string ext, string status)
        {
            // イベントを発火する
            UpdatePresenceEvent?.Invoke(new UpdatePresenceEventArgs(ext, status));
        }

        public delegate void UpdatePresenceEventHandler(UpdatePresenceEventArgs e);

        public class UpdatePresenceEventArgs : EventArgs
        {
            private readonly string _ext;
            private readonly string _status;

            public string ext { get { return _ext; } }
            public string status { get { return _status; } }

            public UpdatePresenceEventArgs(string ext, string status)
            {
                _ext = ext;
                _status = status;
            }
        }

        #endregion

        public WebSocket()
        {

            if (string.IsNullOrEmpty(Properties.Settings.Default.WebSocketUrl))
            {
                new ArgumentNullException("WebSocket URLが設定されていません。");
            }

            var options = new IO.Options();

            //// JWT Token
            //if (Model.WebAPI != null && !string.IsNullOrEmpty(Model.WebAPI.token))
            //{
            //    options.ExtraHeaders.Add("Authorization", string.Format("Bearer {0}", Model.WebAPI.token));
            //}

            socket = IO.Socket(Properties.Settings.Default.WebSocketUrl, options);

            socket.On(Socket.EVENT_CONNECT, (data) =>
            {
                logger.Debug($"EVENT_CONNECT {data}");

                JObject jout = JObject.FromObject(new { channel = "BroadcastChannel" });

                socket.Emit("subscribe", jout);
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                logger.Debug($"EVENT_CONNECT_ERROR {data}");
            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, (data) =>
            {
                logger.Debug($"EVENT_CONNECT_TIMEOUT {data}");
            });

            socket.On(Socket.EVENT_DISCONNECT, (data) =>
            {
                logger.Debug($"EVENT_DISCONNECT {data}");
            });

            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                logger.Debug($"EVENT_ERROR {data}");
            });

            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                logger.Debug($"EVENT_MESSAGE {data}");
            });
           
            // プレゼンス情報の更新イベント
            socket.On("App\\Events\\PresenceUpdated", new PresenceUpdateEvent(this));

        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~WebSocket()
        {

            if (this.socket != null)
            {
                this.socket.Disconnect();
                this.socket.Close();
            }

        }

    }

    /// <summary>
    /// WebSocketでプレゼンス情報更新を受け取る
    /// </summary>
    public class PresenceUpdateEvent : IListener
    {
        WebSocket self = null;

        public PresenceUpdateEvent(WebSocket _self)
        {
            this.self = _self;
        }

        /// <summary>
        /// イベント発生時に呼ばれる関数
        /// </summary>
        /// <param name="args"></param>
        public void Call(params object[] args)
        {

            JObject data = (JObject)args[1];

            var extNumber = data.GetValue("ext").Value<string>();
            var extStatus = data.GetValue("status").Value<string>();

            self?.UpdateProgress(extNumber, extStatus);

        }

        public int CompareTo(IListener other)
        {
            throw new NotImplementedException();
        }

        public int GetId()
        {
            throw new NotImplementedException();
        }
    }

}
